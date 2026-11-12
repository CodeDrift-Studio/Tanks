# Privacy Policy for Tank Mayhem

**Publisher:** CodeDrift Studio
**Game:** Tank Mayhem (Android package ID: `com.cds.tankmayhem`)
**Effective date:** 12 November 2026

---

## 1. Introduction

Tank Mayhem is a single-player tank combat game for Android, published by CodeDrift Studio ("we", "us", "our"). You play against computer-controlled opponents.

This Privacy Policy explains what information the game and the third-party services built into it collect, how it is used, and what choices you have. By installing and playing Tank Mayhem you acknowledge this policy.

In short:

- Tank Mayhem has **no account sign-up, no login screen, no chat or social features, no advertising and no in-app purchases**.
- The game uses **Google Firebase** services. These create an **anonymous Firebase user ID** for your installation and collect **usage (analytics) data and crash reports**.
- We do not ask you for your name, email address, phone number, contacts, photos, microphone or location.
- The Firebase data is tied to identifiers (see Sections 2 to 4). It is **pseudonymous, not anonymous**, and we do not describe it as anonymous.

## 2. Information We Collect

### 2.1 Information you give us directly

The game does not ask you to type in any personal information. If you email us (Section 12), we receive your email address and whatever you write to us, and we use it only to reply to you.

### 2.2 Information collected through the game and its Firebase services

| Category | Collected by | What it contains |
|---|---|---|
| Anonymous user ID | Firebase Authentication | A randomly generated Firebase user ID created the first time the game can connect. No name, email or password is involved. |
| Gameplay and usage events | Firebase Analytics | When a game session starts; when a match starts and ends; which tank you chose; round results; which power-up types you collect. Details in Section 4.2. |
| Crash and diagnostic data | Firebase Crashlytics | Crash reports and error traces, with the app version, the current game state, round number and arena name. Details in Section 4.3. |
| Game configuration | Firebase Remote Config | The game downloads configuration values from our Firebase project. Details in Section 4.4. |
| Device, app and network information | Firebase SDKs (automatic) | Collected automatically by the Firebase SDKs. Details in Section 3. |

### 2.3 Information stored on your device

The game and the Firebase SDKs store some data locally on your device, such as your anonymous sign-in session, cached configuration values, and a stored setting for whether Firebase data collection is on. This data stays on your device until you clear the game's storage or uninstall it.

## 3. Automatically Collected Information

Firebase SDKs collect some information automatically once the game is running and can reach the internet. Based on Google's published Firebase documentation, this typically includes:

- **Identifiers:** a Firebase installation / app-instance ID that identifies this installation of the game, and the Firebase user ID from Section 2.2. The game's Android manifest also declares the advertising-ID permission (`AD_ID`) that Firebase Analytics uses, so the **Google Advertising ID** may be collected where your device provides one and has not been reset or restricted. Tank Mayhem itself shows no ads.
- **Device and app information:** device model, manufacturer, operating system and version, language and region settings, app version, and installation and update events.
- **Approximate location:** Firebase Analytics may derive a country or city-level location from your IP address. The game does not request your GPS or precise location.
- **Network data:** your IP address is received by Google's servers when the game connects to them.
- **Session information:** timestamps, session duration and engagement data.

We do not control exactly which automatic fields Google collects; they are defined by Google and may change with SDK updates. See the documents linked in Section 10.

## 4. Firebase and Third-Party Services

Tank Mayhem uses the following Google Firebase services. Google provides them and processes the resulting data on our behalf and under its own terms.

### 4.1 Firebase Authentication (anonymous)

When the game starts and has an internet connection, it creates an **anonymous Firebase user** for your installation. This produces a random user ID that is stored on your device and on Google's servers. It lets us recognise the same installation across sessions. No personal information is required to create it. If the game is offline, it tries again later.

The anonymous user ID is a persistent identifier tied to your installation. Because it can be linked with the other data described here, it should be treated as personal data, not as anonymous data.

### 4.2 Firebase Analytics

Firebase Analytics collection is **switched on by default**. The game does not currently include a screen that lets you turn it off (see Section 9 for the choices that are available).

Besides the automatic data in Section 3, the game logs these custom events:

| Event | When | Information sent |
|---|---|---|
| `game_session_start` | The game launches | none beyond the automatic data |
| `match_start` | A match begins | number of players, human players and computer opponents; rounds needed to win; arena name |
| `tank_selected` | You choose a tank in the menu | the tank's name |
| `round_complete` | A round ends | round number; result (win, loss or draw) |
| `match_complete` | A match ends | result; rounds played; rounds won by the human player; match duration in seconds |
| `powerup_collected` | You pick up a power-up | the power-up type |

We do not send your name, email address, typed text or the Firebase user ID as an event parameter. Firebase Analytics may nevertheless associate these events with the installation identifiers in Section 3.

### 4.3 Firebase Crashlytics

Crashlytics collection is **switched on by default**. When the game crashes, Crashlytics receives a crash report that can include the stack trace, device model, operating system version, memory and storage state, and whether the device is rooted, together with a Crashlytics installation ID. The game adds these custom values: app version, current game state (for example "MainMenu" or "Game"), round number, and arena name.

### 4.4 Firebase Remote Config

The game downloads configuration values from our Firebase project when it starts (at most about once every 12 hours). The values describe match settings, such as rounds needed to win and power-up respawn time. The game does not send gameplay data to request them, but, as with any network request, Firebase receives the installation identifiers, app version and device information needed to deliver them.

### 4.5 Android permissions

The game's Android manifest declares these permissions, which come from the game and the Firebase components:

- `INTERNET` and `ACCESS_NETWORK_STATE`, to connect to Firebase and detect connectivity.
- `WAKE_LOCK`, used by Firebase components to complete background work.
- `com.google.android.gms.permission.AD_ID`, `ACCESS_ADSERVICES_AD_ID` and `ACCESS_ADSERVICES_ATTRIBUTION`, used by Firebase Analytics for advertising ID and attribution features. Tank Mayhem shows no ads.
- A few system and Google Play services permissions that Google libraries add (for example to read Google Play services configuration and handle install-referrer information).

The game does not request access to your camera, microphone, contacts, photos, files or precise location.

## 5. How We Use Information

We use the information described above to:

- keep the game working, including recognising your installation across sessions;
- find and fix crashes and errors and understand which devices and versions are affected;
- understand how the game is played (for example which tanks and power-ups are popular and how long matches last) so we can balance and improve it;
- adjust game configuration through Remote Config;
- respond to you if you contact us.

We do not use this information for advertising or for automated decisions about you.

## 6. Data Storage and Retention

- **On your device:** local data stays until you clear the game's storage or uninstall the game. Uninstalling or clearing data removes the local sign-in session, so a new anonymous user ID is created if you install the game again.
- **On Google's servers:** Firebase data (the anonymous user record, analytics data, crash reports and Remote Config request data) is stored on Google's infrastructure, which may be located in countries other than yours. How long it is kept is set by Google's policies and by the settings of our Firebase project.
- We have **not set up automatic deletion** of anonymous user records, analytics data or crash reports on a fixed schedule. We delete data when you ask us to (Section 9) or when we decide it is no longer needed.

## 7. Data Sharing and Disclosure

We share information only as follows:

- **Google (Firebase).** The data described in Sections 2 to 4 is sent to and processed by Google as our service provider. Google may also use certain data in line with its own terms and privacy policy, for example to operate and secure its services.
- **Legal and safety.** We may disclose information if required by law, legal process or a valid government request, or to protect rights, safety or the security of the game.
- **Business changes.** If CodeDrift Studio is involved in a merger, acquisition or sale of assets, information may be transferred as part of that transaction.

We do not share data with advertising networks, because the game contains none.

## 8. Children's Privacy and Data Security

**Children.** Tank Mayhem is intended for players aged 13 and over and is not directed at children under 13. We do not knowingly collect personal information from children under 13 (or the higher minimum age that applies in your country). If you are a parent or guardian and believe your child has used the game and data about them has been collected, contact us at the address in Section 12 and we will work to delete it.

**Security.** We rely on the security measures of Google Firebase to protect data in transit and on its servers. No method of transmission or storage is completely secure, and we cannot guarantee absolute security.

## 9. User Rights and Data Deletion Requests

**What you can do today**

- **Stop the data sent by the game:** uninstall it, or turn off its network access in Android settings. The game still works offline.
- **Delete data stored on your device:** clear the game's storage in Android settings, or uninstall the game.
- **Limit advertising-ID use:** in Android settings, go to Privacy or Google, then Ads, to delete or reset your advertising ID or opt out of ad personalisation.
- The game does not currently include an in-game screen to opt out of Analytics or Crashlytics, to view your anonymous user ID, or to delete your anonymous account.

**Asking us to access or delete data**

Depending on where you live, you may have rights to ask what information we hold about you, to correct or delete it, or to object to its use. To make a request, email us at the address in Section 12 with the subject line "Tank Mayhem data request". Please include your device model, the approximate dates you played and the game version, so we can try to find your records. Because the game does not show you your anonymous user ID, and the data is not linked to your name or email, we may not be able to identify records that belong to you from an email alone. We will tell you if we cannot. Where we can identify your records, we will delete the anonymous Firebase user record linked to them. Analytics and crash data held by Google is handled under Google's own processes and retention settings, and we may not be able to remove individual events from it.

## 10. Third-Party Privacy Policies

The Firebase services are provided by Google. Their handling of information is described in:

- Google Privacy Policy: https://policies.google.com/privacy
- Firebase Privacy and Security: https://firebase.google.com/support/privacy

We are not responsible for the content or practices of third-party services, and their policies may change.

## 11. Changes to This Privacy Policy

We may update this policy, for example when we add features or services. We will post the new version at this web address and change the effective date at the top. Please check this page from time to time.

## 12. Contact Information

**CodeDrift Studio**
Email: studio.codedrift@gmail.com
Game: Tank Mayhem (`com.cds.tankmayhem`)

If you have questions about this Privacy Policy or want to make a request described in Section 9, please contact us at the address above.
