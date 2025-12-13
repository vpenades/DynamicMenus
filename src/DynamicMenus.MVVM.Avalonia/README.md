## Dynamic Menu Items

### Overview

Avalonia has a pathological problem with dynamic MenuItems; 

- [1466](https://github.com/AvaloniaUI/Avalonia/issues/1466)
- [2325](https://github.com/AvaloniaUI/Avalonia/issues/2325)
- [MenuFlyout ItemsSource item binding command always disabled](https://github.com/AvaloniaUI/Avalonia/issues/15689)
- [Dynamically Generated MenuItem does not update IsChecked property of bound backing property.](https://github.com/AvaloniaUI/Avalonia/issues/16687)
- [TemplateBinding with ContextMenu ItemsSource does not appear to work](https://github.com/AvaloniaUI/Avalonia/issues/17163)
- [MenuFlyout ItemsSource Binding Does Not Update Visually When Collection Instance Is Replaced (MVVM)](https://github.com/AvaloniaUI/Avalonia/issues/18866)

- [MVVM ContextMenus MenuItems and so on](https://github.com/AvaloniaUI/Avalonia/discussions/19622)

The proposed solutions are not trivial and involve quite a lot of boilerplate code, specially if polymorphism is involved.

This library addresses the issue provinding a collection of ModelView objects and their view counterparts, to support dynamically generated menus.