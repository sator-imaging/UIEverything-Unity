UI Everything
=============
Simplifies UI event handling on Unity.


How to Use
----------

Like as `Awake`, `OnEnable`, `Start` and similar methods in `MonoBehaviour`,  
You only need to implement the following methods in event handler class to make UI event callbacks work.

```csharp
void OnSliderChanged(Slider sender);
void OnButtonClick(Button sender);
void OnToggleChanged(Toggle sender);
void OnInputFieldChanged(InputField sender);
void OnInputFieldChangedInteractive(InputField sender);
void OnScrollbarChanged(Scrllbar sender);
void OnDropdownChanged(Dropdown sender);
void OnScrollRectChanged(ScrollRect sender);
```


Important Note
--------------

If you want to set value to sender from event handler,
use `SetValueWithoutNotify` method in sender class instead of assigning new value to `sender.value`.

Using `SetValueWithoutNotify` doesn't invoke event callback so that
you can prevent potential of callback looping.


On Unity 2019 or earlier, You need workaround to do that.

```csharp
// example: sender is Toggle control
var callbacks = sender.onValueChanged;
sender.onValueChanged = new Toggle.ToggleEvent();
sender.isOn = true;
sender.onValueChanged = callbacks;
```


Copyright
---------

Copyright © 2021 Sator Imaging, all rights reserved.
