# JamStartupKit 2D
A Unity kit of assets and scripts for game jams

### Installation

1. Clone this repo using `git clone https://github.com/alelievr/2D-JamStartupKit`
2. Remove the origin of the repository `git remote remove origin`
3. Add your repository as destination `git remote add origin https://my/git/url`
4. Run Unity and rename the project inside `Project Settings > Player Settings > Product name`
5. Push the initial state of your project `git push --set-upstream origin master`

### Assets included
+ [Post processing stack V2](https://github.com/Unity-Technologies/PostProcessing)
+ [2D Animations](https://docs.unity3d.com/Packages/com.unity.2d.animation@1.0/manual/index.html)
+ [SpriteShape](https://docs.unity3d.com/Packages/com.unity.2d.spriteshape@1.0/manual/index.html)
+ [2D extras (for tilemap)](https://github.com/Unity-Technologies/2d-extras)
+ [DOTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676)
+ [Cinemachine](https://www.assetstore.unity3d.com/en/#!/content/79898)
+ [JamKit](https://github.com/alelievr/JamKit)

### Modified project settings
Chnages done accordingly to [the project setup best practices](https://blogs.unity3d.com/2017/08/10/spotlight-team-best-practices-project-setup/)

+ Fixed timestep to 0.01666
+ Maximum Allowed Timestep to 0.1
+ Gamma color space (for web target else use linear)
+ Graphics jobs false (causes lag spike on old GPUs)
+ GPU Skinning to true
+ Anti aliasing to disabled
+ VSync to disabled
+ Asset serialization to force text
