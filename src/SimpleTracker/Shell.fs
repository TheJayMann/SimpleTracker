module SimpleTracker.Shell

open Elmish
open Avalonia.Controls.Primitives
open Avalonia.Controls
open Avalonia.Layout
open Avalonia.FuncUI.DSL

type Model = {
  ShowCompletedItems: bool
  TrackerItems: TrackerItem list
  Services: Services
}

type Msg =
| NoMsg
| ShowCompletedItems
| HideCompletedItems
| ShowOpenFileDialog
| LoadTrackerItems of TrackerItem list

let mkOptionalMsg f = Option.map f >> Option.defaultValue NoMsg

let init services = 
  let initModel = {
    ShowCompletedItems = false
    TrackerItems = []
    Services = services
  }
  initModel, Cmd.none

let private updateUI msg model = 
  match msg with
  | NoMsg -> model
  | ShowCompletedItems -> { model with ShowCompletedItems = true }
  | HideCompletedItems -> { model with ShowCompletedItems = false }
  | LoadTrackerItems items -> { model with TrackerItems = items }
  | ShowOpenFileDialog -> model

let private dispatchCmd msg model = 
  match msg with
  | NoMsg -> Cmd.none
  | ShowOpenFileDialog -> 
    LoadTrackerItems 
    |> mkOptionalMsg 
    |> Cmd.OfTask.perform model.Services.GetTrackerData ()
  | ShowCompletedItems
  | HideCompletedItems 
  | LoadTrackerItems _ -> Cmd.none

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
    DockPanel.lastChildFill true
    DockPanel.children[
      menu dispatch
      CheckBox.create [
        CheckBox.dock Dock.Top
        CheckBox.content "Show all items"
        CheckBox.isChecked model.ShowCompletedItems
        CheckBox.onUnchecked (fun _ -> dispatch HideCompletedItems)
        CheckBox.onChecked (fun _ -> dispatch ShowCompletedItems)
      ]
      ListBox.create[
        ListBox.dataItems model.TrackerItems
      ]
    ]
  ]

