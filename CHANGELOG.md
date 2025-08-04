## Changelog

All notable changes to this project will be documented in this file.

#### **Version 3.0.0**

> - Breaking Changes:
>	   - Renamed RichItemsControl to RichCanvas
>    - Renamed RichItemContainer to RichCanvasContainer
>    - Renamed RichCanvas to RichCanvasPanel
>    - Removed PanningGrid (scrolling is now handled inside main control class and needs to be wrapped in a ScrollViewer)
>    - Removed RichCanvas.HorizontalScrollBarVisibility dependency property
>    - Removed RichCanvas.VerticalScrollBarVisibility dependency property
>    - Removed RichCanvas.SelectionEnabled dependency property
>    - Removed RichCanvas.EnableNegativeScrolling dependency property
>    - Removed RichCanvas.ExtentSize dependency property
>    - Removed RichCanvas.PanningKey dependency property
>    - Removed RichCanvas.ZoomKey dependency property
>    - Removed RichCanvas.DisableScroll dependency property
>    - Removed RichCanvas.GridStyle dependency property
>    - Removed RichCanvas.TranslateOffset dependency property
>    - Removed Scrolling routed event
>    - Renamed Scale dependency property to ViewportZoom
>    - Replaced ViewportRect to ViewportSize dependency property
> - Features:
>	   - All default operations of RichCanvas work now based on states classes derived from CanvasState class
>    - Interactions between them and input gestures are defined through a state obtained from method RichCanvas.GetDefaultState()
>      - Inheriting RichCanvas and overriding this method will let you define your own states and orchestration of them
>    - Default states:
>      - DefaultState
>      - DrawingState
>      - MultipleSelectionState
>      - SingleSelectionState
>      - PanningState
>	  - All default operations of RichCanvasContainer work now based on states classes derived from ContainerState class
>    - Interactions between them and input gestures are defined through a state obtained from method RichCanvasContainer.GetDefaultState()
>      - Inheriting RichCanvasContainer and overriding this method will let you define your own states and orchestration of them
>    - Default states:
>      - ContainerDefaultState
>      - DraggingContainerState
>    - Added configurable input gestures for RichCanvas and RichCanvasContainer default offerd operations (through states) to RichCanvasGestures
>    - Added CurrentState, PushState and PopState to RichCanvas and RichCanvasContainer
>    - Added MultiGesture utlity for combining multiple input gestures into one gesture
>    - Added MouseKeyGesture utility for combining a MouseGesture with a Key[] or KeyGesture[] into one gesture
>    - Added UI Automation framework AutomationPeer classes for RichCanvas and RichCanvasContainer for discoverability when using UI Automation tools based on UIA framework from Microsoft
>    - Added RichCanvasContainer.AllowScaleChangeToUpdatePosition dependency property to control the position update when scaling is changed.
>    - Changed IsPanning property on RichCanvas from internal to public
>    - Added RichCanvas.DrawingEndedCommand dependency property invoked after an item has been drawn
>    - Added RichCanvas.ViewportLocation depenedency property to get or set the viewport's top-left coordinates in graph space coordinates
>    - Added RichCanvas.ItemsExtent dependency property to get the area covered by the RichCanvasContainers present on RichCanvas
> - Bugfixes:
>    - Items added to the ItemsSource that are valid for drawing are now handled in the correct order and reacting to any changes like removing or moving
