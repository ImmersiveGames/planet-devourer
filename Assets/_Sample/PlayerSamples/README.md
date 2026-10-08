# Player Samples

## Purpose

Este índice aponta para composições consumer atuais de Player no projeto. Os samples demonstram provisionamento, Actor selection, input, ciclo Join/Leave/Rejoin e diferentes modelos de Camera. Cada README local descreve sua composição concreta; este arquivo não substitui os guias do Framework.

## Samples

| Sample | Contrato demonstrado | Assets centrais | Status registrado |
|---|---|---|---|
| [Manager-Provisioned](ManagerProvisioned/README.md) | Join materializa Local Player Host e Actor; Assignment IndividualPerPlayer segue o Slot | `GameApplication_ManagerProvisioned`, `PlayerSessionProfile_ManagerProvisioned`, `PlayerSlotProfile_ManagerProvisioned`, `CameraAssignment_ManagerProvisioned_ThirdPerson` | README instrui validação manual; conferir status atual antes de certificar |
| [Joining Control](JoiningControl/README.md) | Open/Close Joining, Join/Leave/Rejoin e manutenção da participação do Player | `GameApplication_JoiningControl`, `PlayerSessionProfile_JoiningControl`, `CameraAssignment_JoiningControl_ThirdPerson` | composição migrada; consultar evidência de execução no tracker/sample |
| [Character Selection](CharacterSelection/README.md) | Escolha explícita entre Actor Profiles com Actor runtime único | `GameApplication_CharacterSelection`, `PlayerSessionProfile_CharacterSelection`, Farmer/Cow Actor Profiles, `CameraAssignment_CharacterSelection` | Manual Play Mode PASS registrado no README local |
| [Local Multiplayer](LocalMultiplayer/README.md) | Dois Slots Manager-Provisioned, SharedGroup e bloqueios de gameplay independentes por Player | `GameApplication_LocalMultiplayer`, dois Slot Profiles, `CameraAssignment_LocalMultiplayer_Group` | SharedGroup PASS 2026-10-06; trigger IF-ADR-044 consumer PASS 2026-10-08 |
| [Character Selection Multiplayer Split Screen](CharacterSelectionMultiplayerSplitScreen/README.md) | Dois Players, escolha independente de Actor e Outputs IndividualPerPlayer | `GameApplication_CharacterSelectionMultiplayerSplitScreen`, dois Slot Profiles, `CameraAssignment_CharacterSelectionMultiplayer` | Manual Play Mode PASS registrado no README local |

`GettingStarted/MinimalGame` é o exemplo canônico Scene-Provided e está documentado no [índice Getting Started](../GettingStarted/README.md). Não crie outro sample Scene-Provided para repetir a mesma composição sem um contrato de consumidor distinto.

## Current Player ownership model

```text
GameApplicationAsset
  PlayerSessionProfile
    supported PlayerSlotProfiles

Player Session
  owns logical Session membership and Player lifetime

Local Player Host
  owns PlayerInput boundary and ActorMount

Actor occurrence
  owns physical/spatial state, gameplay input reader and explicit Camera Subject

Activity
  owns contextual participation, preparation/readiness and relocation policy
```

Scene-Provided authors the Local Player Host and Actor occurrence in the gameplay scene for Framework resolution/adoption. Manager-Provisioned creates the Host through its explicit provisioning composition, then resolves and prepares the selected Actor. Neither mode transfers actor occurrence ownership to decorative visual content.

`PlayerSessionProfile` defines initial Session provisioning intent. `PlayerSlotProfile` owns stable Slot identity and optional default Actor intent. `ActorProfile` describes reusable Actor identity/classification and optional visuals; it does not own a physical Actor occurrence.

## Camera ownership used by current samples

Current samples configure `SessionCameraAssignmentAsset` in `GameApplicationAsset.StartupCameraAssignments`:

- `CameraAssignment_LocalMultiplayer_Group`: `SharedGroup`, explicit P1/P2 Slots, member Actor Subjects, one `CameraOutput_Main` and a Group rig.
- `CameraAssignment_ManagerProvisioned_ThirdPerson`: `IndividualPerPlayer`, explicit Slot-to-Output map and Third Person rig.
- `CameraAssignment_CharacterSelection`: per-Player Fixed Follow rig and explicit Slot mapping.
- `CameraAssignment_CharacterSelectionMultiplayer`: per-Player mapping to two physical Outputs; `PlayerInputManager` owns viewport geometry.

The Session owns Assignment activation/replacement/clear, occurrence lifetime, membership/Subject projection, Output routing and fallback coverage. Route/Activity changes can affect eligibility through Player context, but Route and Activity assets do not own Assignment selection. Actor occurrences supply exact `ActorCameraSubjectAuthoring` observations. An Output prefab owns the Unity Camera, Cinemachine Brain and fallback rig through `CameraOutputAuthoring`; `CameraRigComposer` materializes its local rig.

## Runtime Gameplay Availability

Local Multiplayer demonstrates `PlayerGameplayAvailabilityBlockTrigger` for P1 and P2. Each component has Activity scope and an explicit Slot Profile, lives in Activity-owned `LocalMultiplayerUI` content, and owns/releases only its own occurrence-scoped token through `RequestBlock()` / `RequestRelease()`.

Scope means lifecycle ownership. Merely placing a consumer in a scene does not give it Activity ownership. An Activity-scoped `PlayerSessionScopedAccessConsumer` must be in content discovered by that Activity. Arbitrary Persistent Content does not receive Activity binding.

The trigger changes gameplay input availability only. It does not change Joined/GameplayReady state, Actor, devices, camera membership, or gameplay rules. Physical input posture remains projected by the Framework gate adapter; consumers must not write `PlayerInput` or its gameplay Action Map directly.

## Validation boundary

Status labels apply only to the behavior explicitly exercised. Current sample evidence recorded in local READMEs includes:

- Local Multiplayer SharedGroup manual Play Mode PASS on 2026-10-06;
- Local Multiplayer IF-ADR-044 consumer manual PASS on 2026-10-08;
- Character Selection manual Play Mode PASS;
- Character Selection Multiplayer Split Screen manual Play Mode PASS.

Framework tracker evidence separately records Framework EditMode 175/175, Camera Editor 78/78, IF-ADR-042 focused QA 7/7, IF-ADR-043 focused QA 7/7 and QA-NEW-004 continuity 9/9. None alone certifies every Player sample, package import, release readiness or broader IF-ADR-038 recertification. QAFramework owns exhaustive synthetic coverage; these samples remain consumer demonstrations.
