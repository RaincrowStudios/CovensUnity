

#####   Instant Mask   #####

簡単で滑らかな階調のMask!
MeshRendererやSpriteRenderer、UnityGUI、Particle等のイメージを
もう一枚の画像の形に切り取ったり、逆にその形に穴を空けたりします
アルファまたはグレイスケール画像を使った滑らかな階調のマスクで
回転やマスク適用度などの調整も可能です



========== 使用方法 ==========


用途に応じた3つのスクリプトをGameObjectにAddして使用します


- InstantTextureMask
設定値の変更をあまり頻繁に行わない一枚画像用途で使用します
マスクした画像を動的に作り、結果画像はResultに生成されます
例)Texture2D MaskedTex = GetComponent<InstantTextureMask>().Result;
UnityGUIのImage, RawImageコンポーネントと同じGameObjectに設置した場合には
自動で画像をImage, RawImageコンポーネントに適用し表示できます
Unityの仕様として、画像は'Read/Write Enabled'をONにしないと処理に使用することができないので
必ずONにしてください
(画像を選択しInspectorウインドウでTexture TypeをAdvancedにすると現れる項目です)

また、ExportResultImageボタンを押せば結果画像をファイルとして書き出すことができます
ProjectウインドウにおけるInstantMask/ExportImages内に、年月日時分秒+4桁のナンバーという
数字ファイル名で生成されます



- InstantShaderMask
シェーダーを使って各RendererやGUIのテクスチャをマスクします
マスク部分を頻繁に移動や回転させるような場合に使用します

これを設置したGameObjectが持つRendererやUnityGUIのImage, RawImageコンポーネントに
InstantMaskShader.shaderを当てたマテリアルをアサインしてください
このマテリアルを通してマスクした結果の画像を描画します



- InstantParticleMask
基本的な構造は上のInstantShaderMaskと同じですが、
パーティクルでのテクスチャおよびテクスチャシートアニメーションをマスクします

これを設置したGameObjectが持つParticle(Shuriken)のRendererに
InstantMaskShaderを当てたマテリアルをアサインしてください
このマテリアルを通してマスクした結果の画像を描画します




========== 各設定 ==========

Inspectorウインドウでの各設定項目の意味は、設定名でおおよそ把握できると思います
設定名はそのまま変数名になっています(大文字小文字はそのままで空白スペースを埋めた文字列)
例えばMaskの角度(MaskAngle)をスクリプトから変えたい場合
例) GetGomponent<InstantShaderMask>().MaskAngle += 2.0f;
とUpdate()内で変更させれば2度づつ回転します
デモで使用しているIMaskDemoManager.csを参考にしてください
あるいはAnimationウインドウのタイムラインで変化させてもかまいません

各 WrapMode と SelectMaterial で使われているドロップダウンリストは、内部的にはint型なので
スクリプトから変更する場合はリストの上から順に0からの整数を指定すれば変更できます
例) GetGomponent<InstantShaderMask>().MaskImageWrapMode = 1;
これでMask画像のWrapModeはClampになります

InstantTextureMask のサイズや位置の変更はPixel単位、サイズや位置や回転のPivotはマスク画像の中心、
マスク位置の座標原点(x:0,y:0)はベースの画像の左下です
InstantShaderMask, InstantParticleMask のマスクサイズ変更はUnityのシェーダーの設定に
準拠しておりTilingで行います
回転のPivotはMask画像の中心です

更に詳しく設定や挙動を知りたい場合は、このテキストの末尾にある解説サイトを見てください




========== Unityの仕様に伴う細かい注意点 ==========

* WrapModeは元となる画像の設定として行うもののため、Edit時(シーンPlayしていない状態)では
  このアセットからは変更できないようになっています
  Edit時は元の画像の設定から通常の方法でWrapModeの変更を行って下さい
  ビルドしたゲーム内やシーンPlay中にはこのアセットからWrapModeを変更する事ができます
  ただし、先述の通りWrapModeは元の画像の設定なので、同じ画像を使用しているGameObject全てが一斉に
  WrapModeの変更を受けることに注意して下さい


* InstantMaskShader.shaderはUnity準拠のUI用シェーダーにプロパティを付け加えたものです(UI-Default.shader)
  あなたのお好みのシェーダーを使ってマスクしたい場合には、その別のシェーダースクリプトに
  InstantMaskのプロパティが適用できるようにあなた自身が書き加える必要があります
  プログラムからシェーダーを適切に書き変えるようなことはできないため、
  ユーザー側で手動で行ってもらうしかないためです


* Unity自体の仕様におけるマテリアルの挙動の話ですが、
  ビルドしたゲームやEditorでのシーンPlay時には、Rendererで同一のマテリアルが
  共有されていても自動的にInstanceが作られ、各々のGameObjectで個別のレンダリングが行われます
  この場合はいいのですが、Edit時(シーンPlayしていない状態)にはInstanceが作られない仕様のため
  同じマテリアルを共有しているGameObjectには一斉にマテリアルへの変更が適用されてしまいます
  そのため同じマテリアルを共有する複数のGameObjectの個別のMask状態を見たい場合、
  シーンPlayして確認する必要があります

  また、UnityGUIのImage, RawImageコンポーネントにおいてはUnityはマテリアルのInstanceを作らないので、
  ビルドしたゲームやEditorでのシーンPlay時であっても、同じマテリアルは全て同一の描画結果に
  なることに注意が必要です

  これらの挙動に対して、もしこのアセットがマテリアルを複製等して解決してしまうと
  他のアセットなどからマテリアルを参照している場合に挙動の食い違いによる
  エラー等が懸念されるため、Unity標準の挙動のままにしています
  各GameObjectに全て個別のマテリアルを使用している場合にはこの問題ありません



===============================================================


Instant Maskの詳しい使い方、更なる説明についてはサイトを見てください!
http://kakatte.webcrow.jp/imask/index.html

製作者: Myouji



