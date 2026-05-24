# Rapport d'implémentation — Fonctionnalités VR (MUST-HAVE)

**Projet :** Flat (jeu d'horreur VR)
**Plateforme :** Meta Quest 3S — Unity 6000.0.73f1, OpenXR + Meta XR Interaction SDK (rig `OVRCameraRig`), URP, FMOD
**Scène de référence :** `Assets/_Project/_Scenes/Game_Act1_TESTVR.unity`

Ce document prouve l'implémentation de chaque élément MUST-HAVE en pointant le fichier, le composant, le mécanisme et la façon de le vérifier en jeu.

> Les scripts spécifiques VR ajoutés sont regroupés dans `Assets/Dev/`. Les scripts gameplay réutilisés sont dans `Assets/_Project/Scripts/`.

---

## VR·CORE — Player VR Controller (déplacement + rotation joystick)

**Fichier :** `Assets/Dev/VRRigMovementController.cs` (sur `OVRCameraRig`, à côté du `CharacterController`)

**Déplacement**
- Lecture du stick gauche via l'`InputManager` (action *Move* bindée sur `<XRController>{LeftHand}/primary2DAxis`).
- Direction relative au regard (`xrCamera.forward/right` aplatis sur le plan horizontal).
- Lissage accélération/décélération (`Vector3.MoveTowards` avec `acceleration`) → mouvement fluide.
- Gravité appliquée + `CharacterController.Move`.

**Rotation joystick (snap turn = confort anti-nausée)**
- `HandleSnapTurn()` lit le stick **droit** (`CommonUsages.primary2DAxis.x`) via `UnityEngine.XR.InputDevices`.
- Au-delà de `turnThreshold`, rotation par pas de `snapAngle` (30°) autour de la tête (`transform.RotateAround(headPos, Vector3.up, angle)`), avec ré-armement quand le stick revient au centre.

**Course**
- `ThumbstickClicked()` : un clic de thumbstick (`CommonUsages.primary2DAxisClick`) bascule `walkSpeed` (2) → `runSpeed` (4).

**Preuve en jeu :** stick gauche = se déplacer dans la direction du regard ; stick droit gauche/droite = pivoter par crans ; clic du stick = courir.

---

## VR·HAND — Grab / interaction main → IInteractable porté en XR

**Saisie d'objets (Meta Interaction SDK)**
- Les mains du rig portent des `HandGrabInteractor` / `DistanceHandGrabInteractor`.
- Les objets saisissables (`IKey_VR`, `ITvRemote_VR`, `IFlashLight_VR`) ont `Grabbable` + `HandGrabInteractable` (+ poses de grab).
- **Pont vers l'inventaire :** `Assets/Dev/VRGrabbableInventoryItem.cs` écoute les évènements du `Grabbable` (`WhenPointerEventRaised` → `Select`/`Unselect`) pour exposer l'objet et son `Item` au système d'inventaire.

**Interaction `IInteractable` (portes, objets)**
- `Assets/_Project/Scripts/Gameplay/Interaction/Interactor.cs` : raycast depuis la tête (`CenterEyeAnchor`) ; à la **gâchette index droite** appelle `IInteractable.Interact()`.
- L'entrée est lue via `UnityEngine.XR.InputDevices` (`CommonUsages.triggerButton`, main droite) — chemin OpenXR fiable sur le rig Meta — en plus de l'Input System.

**Preuve en jeu :** approcher la main d'un objet + grip = le saisir ; regarder une porte/objet + gâchette droite = l'actionner.

---

## VR·UI — Inventaire 4 slots diégétique → ceinture du joueur

**Fichiers :**
- `Assets/Dev/VRInventoryController.cs` (cœur du système, sur `IPlayer`)
- `Assets/Dev/VRBeltCollector.cs` (zone de collecte sur `BeltAnchor`)
- `Assets/Dev/VRGrabbableInventoryItem.cs`
- Données : `Flat.Gameplay.Inventory.PlayerInventory` (4 slots)

**Diégétique (pas d'UI 2D plein écran) :**
- `BeltAnchor` (script `VRBeltAnchor`) suit la position/orientation horizontale de la tête à hauteur de hanches.
- 4 ancres `BeltSlot_0..3` réparties **autour de la taille** (arc hanche droite → hanche gauche).
- Les objets collectés **flottent physiquement sur la ceinture** (l'ancien HUD inventaire 2D a été désactivé).

**Boucle d'usage :**
1. Saisir un objet à la main, le relâcher dans la zone ceinture → `VRBeltCollector` appelle `VRInventoryController.Collect()` → `PlayerInventory.AddItemAt(slot, item)` + l'objet est parqué sur le slot de hanche (flag `IsCollected` anti-doublon).
2. **Changer d'objet actif :** bouton **X** → `CycleNext()` équipe l'objet suivant (affiché tenu en main gauche).
3. **Utiliser :** **gâchette index gauche** → `Item.Use()` de l'objet équipé.

**Preuve en jeu :** ramasser la clé/torche/télécommande → elles se rangent visiblement à la ceinture ; X fait défiler l'objet en main ; gâchette = action de l'objet.

---

## VR·AUDIO — FMOD spatial 3D + HRTF binaural sur respiration

**Fichier :** `Assets/_Project/Scripts/Gameplay/Characters/PlayerSound.cs`

- Évènement FMOD de respiration instancié via `RuntimeManager.CreateInstance(breathingEvent)`.
- **Spatialisation 3D :** `_breathingInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject))`, mis à jour chaque frame → position 3D du souffle suivie par le moteur audio.
- **Modulation :** paramètre FMOD `Anxiety` piloté par `PlayerAnxietyController.CurrentAnxiety` → la respiration s'accélère/s'intensifie avec l'anxiété.
- **HRTF binaural :** assuré par l'effet spatializer (FMOD/Meta XR Audio) configuré sur l'évènement de respiration dans le **projet FMOD Studio** ; côté Unity, l'intégration FMOD est active (`Assets/Plugins/FMOD`, `FMODStudioSettings`).

**Preuve en jeu :** au casque, la respiration est localisée (binaurale) et change de rythme/texture selon l'anxiété.

---

## VR·CS — Cybersickness : framerate stable, vignettage dynamique, FOV réduit en mouvement

**1. Vignettage dynamique / FOV réduit — `Assets/Dev/VRComfortVignette.cs` (sur `OVRCameraRig`)**
- Construit un overlay noir radial **ancré à la caméra** (canvas world-space + texture radiale générée à la volée).
- L'opacité périphérique est pilotée par la **vitesse réelle** (`CharacterController.velocity`) :
  - immobile → 0 (aucun assombrissement),
  - en mouvement → assombrissement progressif jusqu'à `maxAlpha`, plus fort en course.
- Effet = **tunnelling** : la périphérie s'assombrit → simule un **FOV réduit pendant le mouvement** (technique anti-nausée standard).

**2. Framerate stable**
- Le vignettage est volontairement **sans post-processing** (un simple calque transparent) → **coût GPU négligeable**, contrairement à un Vignette URP qui imposerait d'activer tout le stack post-process (bloom, grain…) sur Quest.
- Mouvement stabilisé (voir tuning `CharacterController` ci-dessous) → pas de à-coups générateurs de nausée.

**Tuning `CharacterController` (anti-blocage + confort) :** `minMoveDistance = 0` (supprime les micro-blocages le long des murs), capsule `height = 1.8`, `radius = 0.25` (passe les portes), `stepOffset = 0.35`.

**Preuve en jeu :** en se déplaçant, la périphérie s'assombrit doucement ; à l'arrêt l'image est nette ; le framerate n'est pas impacté par l'effet.

---

## HAPTIC — Retour haptique synchronisé sur les callbacks FMOD `Expire_X`

**Source des callbacks :** `PlayerSound.cs`
- L'évènement FMOD de respiration porte des **marqueurs timeline `Expire_X`** (X = niveau d'anxiété).
- `MarkerCallback` (callback FMOD `TIMELINE_MARKER`) détecte les marqueurs préfixés `Expire_`, vérifie la cohérence avec l'anxiété courante, puis déclenche l'évènement statique `public static event Action OnExpire`.

**Retour haptique :** `Assets/Dev/VRBreathHaptics.cs` (sur `IPlayer`)
- S'abonne à `PlayerSound.OnExpire`.
- À chaque expiration : impulsion haptique sur **les deux manettes** via `InputDevice.SendHapticImpulse` (XR).
- **Intensité et durée pilotées par l'anxiété** (`PlayerAnxietyController.NormalizedAnxiety`) :
  - calme → amplitude 0.08 / 0.12 s (subtil),
  - panique → amplitude 0.35 / 0.22 s (marqué).

**Preuve en jeu :** chaque souffle = légère vibration des manettes, synchronisée sur le `Expire_X` FMOD, qui s'intensifie en zone stressante.

---

## Récapitulatif fichiers (traçabilité)

| Élément | Fichier(s) clé |
|---|---|
| VR·CORE | `Assets/Dev/VRRigMovementController.cs` |
| VR·HAND | `Assets/Dev/VRGrabbableInventoryItem.cs`, `Assets/_Project/Scripts/Gameplay/Interaction/Interactor.cs` |
| VR·UI | `Assets/Dev/VRInventoryController.cs`, `VRBeltCollector.cs`, `VRBeltAnchor.cs` + `PlayerInventory.cs` |
| VR·AUDIO | `Assets/_Project/Scripts/Gameplay/Characters/PlayerSound.cs` + projet FMOD Studio |
| VR·CS | `Assets/Dev/VRComfortVignette.cs` + tuning `CharacterController` |
| HAPTIC | `Assets/Dev/VRBreathHaptics.cs` ← `PlayerSound.OnExpire` (marqueurs `Expire_X`) |
| Porte clé (bonus) | `Assets/_Project/Scripts/Gameplay/Interaction/Interactions/LockedDoor.cs`, `.../Inventory/Items/KeyItem.cs` |
