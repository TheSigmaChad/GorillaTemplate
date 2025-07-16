# Economy Setup Guide
This guide will help you set up the economy system. The economy system allows players to purchase and use virtual currency to buy cosmetics.

## How it works
The economy system uses the Meta Horizon Store and PlayFab.
* The Meta Horizon Store is not only how you release your game to players, but it is where you set up in-app purchases of things like virtual currency. It is here that users spend real money.
* PlayFab is a service that allows you to handle purchases of in-game items such as cosmetics using virtual currencies, and keep track of what players have purchased.

## Setup
Before getting started, make sure you have completed the [basic setup](../~README.md#Setup) of the template project.

There are three main steps to set up the economy system:
1. Set up the Meta Horizon Store app.
2. Set up the PlayFab catalog.
3. Configure the Unity project with the Meta Horizon Store App ID and PlayFab Title ID.

### Meta Horizon Store
1. Create a Meta Horizon developer account [here](https://developers.meta.com/horizon/manage/onboarding/). You can use an existing Meta account, or create a new one.
2. Open the [developer dashboard](https://developers.meta.com/horizon/manage).
3. Click **Create a new app**. Enter a name for your app and select **Meta Horizon Store** as the platform, then click **Create**.
4. Open **Development > API** on the left sidebar.
   1. Note the **App ID** in the middle of the page. You will need this in Unity later.
   2. If needed, verify your organization by clicking the **Start verification** button and follow the instructions.
   3. Complete the **Data Use Checkup** by clicking the **Certify for access here**. This is required for your app to access the necessary platform features.
      1. Add **User ID**, **User Profile**, and **In-App Purchases and/or Downloadable Content**. For **Usage** select `Use Add-ons: Downloadable Content and In-App Purchases Commerce (IAP)`. For Description, enter `Used by PlayFab for purchases of virtual currency and cosmetics.`
      2. **NOTE:** It is up to you to ensure that your app uses Meta's platform features as you describe in compliance with Meta's policies.
      3. Click **Submit recertification** to apply your changes.
      4. Answer the remaining questions and click **Submit**. Since PlayFab will store user data, you must declare it as a data processor.
      5. Once submitted, it will take Meta some time to approve your changes.
5. Open **Monetization > Add-ons** on the left sidebar.
   1. Click **Create Add-on**. An add-on is a purchasable item in the Meta Horizon Store.
   2. Set **SKU** to `bananas-1000`. This is the SKU that will be used to purchase 1000 units of a virtual currency we'll create in PlayFab called "Bananas".
   3. Set **Add-on Type** to `Consumable`.
   4. Leave **Show in Store** unchecked.
   5. Under **Pricing**, set **Price** to whatever you want. You'll need to submit payment in order to set the price.
   6. Under **Metadata**, set **Name** to `1000 Bananas`.
   7. Click **Publish** to make the add-on available for purchase.

### PlayFab
1. Create a PlayFab account [here](https://developer.playfab.com/en-us/r/sign-in).
2. Click **My Game**.
3. Open **Settings** on the left sidebar, then select **API Features** at the top.
   1. Note the **Title ID** under **API Access**. You will need this in Unity later.
   2. Under **Enable API Features** check **Allow client to add virtual currency**.
   3. Under **Disable Player Creations**, uncheck **Disable player creation using Client/LoginWithCustomId**.
4. Open **Economy** on the left sidebar, then select **Currency (Legacy)** at the top.
   1. Click **New currency**.
   2. Set **Currency code** to `BN`.
   3. Set **Display name** to `Bananas`.
   4. Set **Initial deposit** to `500`. This is the amount of virtual currency players will start with. You can change this as desired.
5. Open **Economy** on the left sidebar, then select **Catalogs (Legacy)** at the top.
   1. Click **Upload JSON**.
   2. Navigate to the template project folder, and select `GorillaTemplate/PlayFab/PlayFab-Cosmetics.json`

### Unity
1. Open the Unity project.
2. From the top toolbar, select **Meta > Platform > Edit Settings**. Make sure the **Inspector** window is open.
   1. At the top, enter the **App ID** you noted from the Meta Horizon Store app into both the **Oculus Rift** and **Meta Quest** fields.
   2. Below, expand **Unity Editor Settings** and check both **Use Standalone Platform** and **Use Meta Quest App ID over Rift App ID in Editor**.
      1. Enter your Meta developer account credentials in the fields below (email and password) to log in.
3. From the **Project** window navigate to `PlayFabSDK/Shared/Public/Resources/PlayFabSharedSettings.asset` and select it. Open the **Inspector** window.
   1. Enter the **Title ID** you noted from PlayFab into the **Title ID** field.

Now, everything should be set up to work with the template project.

## Testing
You should now be able to purchase cosmetics while running the template project in the Unity editor. In order to test purchasing virtual currency from the Meta Horizon Store, you will need to build the project and run it on a Meta Quest device. Note that developer or test accounts will see a price of $0.01 for non-free items, while other players will see the real price.

It is possible to test credit card transactions without a real credit card by setting up [test users in the Meta Horizon Store](https://developers.meta.com/horizon/resources/test-users#creating-test-users).
