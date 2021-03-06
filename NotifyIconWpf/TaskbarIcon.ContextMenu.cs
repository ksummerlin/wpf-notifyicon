﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification.Interop;

namespace Hardcodet.Wpf.TaskbarNotification
{
  public partial class TaskbarIcon
  {
    /// <summary>
    /// Displays the <see cref="System.Windows.Controls.ContextMenu"/> if
    /// it was set.
    /// </summary>
    private void ShowContextMenu(Point screenPosition)
    {
      if (IsDisposed) return;

      //raise preview event no matter whether context menu is currently set
      //or not (enables client to set it on demand)
      var args = RaisePreviewTrayContextMenuOpenEvent();
      if (args.Handled) return;

      var contextMenu = ContextMenu;

      if (contextMenu != null)
      {
        var router = new CommandRerouter(this);
        router.AttachCommandHandlers(contextMenu);

        CommandRerouter.AttachOneShotCommandHandler
        (
          contextMenu,
          ContextMenu.ClosedEvent,
          (sender, e) => router.DetachCommandHandlers(contextMenu)
        );

        CommandManager.InvalidateRequerySuggested();

        //use absolute position
        contextMenu.Placement = PlacementMode.AbsolutePoint;
        contextMenu.HorizontalOffset = screenPosition.X;
        contextMenu.VerticalOffset = screenPosition.Y;
        contextMenu.IsOpen = true;

        //activate the message window to track deactivation - otherwise, the context menu
        //does not close if the user clicks somewhere else
        WinApi.SetForegroundWindow(messageSink.MessageWindowHandle);

        //bubble event
        RaiseTrayContextMenuOpenEvent();
      }
    }
  }
}
