module SimpleTracker.Shell

open Elmish
open Avalonia.Controls.Primitives
open Avalonia.Controls
open Avalonia.Layout
open Avalonia.FuncUI.DSL

type Model = {
  ShowCompletedItems: bool
  TaskList: TaskList option
  Services: Services
}

type Msg =
| NoMsg
| ShowCompletedItems
| HideCompletedItems
| ShowOpenFileDialog
| LoadTaskList of TaskList

let mkOptionalMsg f = Option.map f >> Option.defaultValue NoMsg

let init services = 
  let initModel = {
    ShowCompletedItems = false
    TaskList = None
    Services = services
  }
  initModel, Cmd.none

let private updateUI msg model = 
  match msg with
  | NoMsg -> model
  | ShowCompletedItems -> { model with ShowCompletedItems = true }
  | HideCompletedItems -> { model with ShowCompletedItems = false }
  | LoadTaskList list -> { model with TaskList = Some list }
  | ShowOpenFileDialog -> model

let private dispatchCmd msg model = 
  match msg with
  | NoMsg -> Cmd.none
  | ShowOpenFileDialog -> 
    LoadTaskList 
    |> mkOptionalMsg 
    |> Cmd.OfTask.perform model.Services.GetTrackerData ()
  | ShowCompletedItems
  | HideCompletedItems 
  | LoadTaskList _ -> Cmd.none

let update msg model = updateUI msg model, dispatchCmd msg model

let private menu dispatch = 
  Menu.create [
    Menu.dock Dock.Top
    Menu.viewItems [
      MenuItem.create [
        MenuItem.header "_File"
        MenuItem.viewItems [
          MenuItem.create [
            MenuItem.header "_Open"
            MenuItem.onClick (fun _ -> dispatch ShowOpenFileDialog)
          ]
        ]
      ]
    ]
  ]

let view model dispatch =
  DockPanel.create [
    DockPanel.verticalAlignment VerticalAlignment.Stretch
    DockPanel.horizontalAlignment HorizontalAlignment.Stretch
    DockPanel.lastChildFill model.TaskList.IsSome
    DockPanel.children[
      menu dispatch
      if model.TaskList.IsSome then
        let taskList = model.TaskList.Value
        TextBlock.create [
          TextBlock.dock Dock.Top
          TextBlock.text taskList.Name
        ]
        CheckBox.create [
          CheckBox.dock Dock.Top
          CheckBox.content "Show all items"
          CheckBox.isChecked model.ShowCompletedItems
          CheckBox.onUnchecked (fun _ -> dispatch HideCompletedItems)
          CheckBox.onChecked (fun _ -> dispatch ShowCompletedItems)
        ]
        ListBox.create [
          ListBox.dataItems taskList.Items
        ]
    ]
  ]

