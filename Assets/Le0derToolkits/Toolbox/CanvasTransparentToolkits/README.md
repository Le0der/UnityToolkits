# **Unity WebGL画布透明**

​	在当今的Web开发领域，Unity作为一个强大的游戏引擎，不仅限于创建桌面或移动平台的游戏，它还通过WebGL为浏览器带来了丰富的交互式3D内容。Unity WebGL允许开发者将游戏或应用直接嵌入网页中，实现跨平台的无缝体验。然而，在这个过程中，如何实现画布（Canvas）元素的透明效果，让Unity内容与网页背景完美融合，成为许多开发者关注的焦点。本文将深入探讨Unity WebGL中实现画布透明的技巧和步骤，帮助你打造出既美观又功能强大的WebGL应用。

## 一、理解Unity画布与WebGL渲染基础

### 1. Unity画布概述

在Unity中，`Canvas`是用于UI元素布局和渲染的基础组件。它支持三种渲染模式：屏幕空间-覆盖、屏幕空间-摄像机、世界空间，不同的模式适用于不同的透明需求和场景布局。

### 2. WebGL渲染上下文

WebGL是一种基于OpenGL ES 2.0的JavaScript API，它允许在网页上进行硬件加速的3D图形渲染。Unity通过将游戏内容编译成WebAssembly（以前是JavaScript）并利用WebGL接口，实现在浏览器中的渲染。透明效果的实现，需要深刻理解WebGL的渲染流程和限制。

## 二、实现Unity WebGL画布透明的关键步骤

{% note info  flat %}

注意：本功能实现需要unity版本2021及以上。

{% endnote %}

1. 在工程的Asset目录下创建一个`Plugins`文件夹；

2. 在`Plugins`文件夹里面创建一个文件，`__TransparentCanvas.jslib`

3. `jslib`文件内容如下：

   ```jslib
   var LibraryGLClear = {
       glClear: function(mask)
       {
           if (mask == 0x00004000)
           {
               var v = GLctx.getParameter(GLctx.COLOR_WRITEMASK);
               if (!v[0] && !v[1] && !v[2] && v[3])
                   // We are trying to clear alpha only -- skip.
                   return;
           }
           GLctx.clear(mask);
       }
   };
   mergeInto(LibraryManager.library, LibraryGLClear); 
   ```

   ​	这个LibraryGLClear对象里面包含了一个glClear方法，这是让网页产生透明结果必须提供的一个方法，这个方法应该是Unity渲染过程中调用的方法。如果有必要，我们也可以在LibraryGLClear对象里面添加更多的方法，不过这不在本文讨论范围内。

4. 渲染画面使用的摄像机的`Environment`菜单中将`Background Type`设置为`Solid Color`, `Background` 颜色中的透明通道 Alpha设置为0

5. 在发布的.html文件中修改canvas的背景属性如下：

   ``` html
   <canvas id="unity-canvas" width=960 height=600 style="width: 960px; height: 600px; background: transparent"></canvas>
   ```

   ​	添加或修改这一行里面的 background: transparent 这个属性设置，默认发布出来background的值不是transparent，必须改成transparent才可以。

## 三、常见问题与解决方案

## 1.使用URP渲染管透明无效

- BUG症状：使用URP渲染管道的时候透明[canvas](https://so.csdn.net/so/search?q=canvas&spm=1001.2101.3001.7020)效果无效
- 原因：HDR和透明效果冲突
- 暂行解决方法：关闭所用的Quality设置的HDR。或者直接设置Quality为Medium或Low

{% note warning flat %}

注：因为关闭了HDR，bloom等相关效果会无效化，仍需更好的方法

{% endnote %}
