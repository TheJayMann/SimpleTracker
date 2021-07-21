module SimpleTracker.Shell

open Elmish
open Avalonia.Controls.Primitives
open Avalonia.Controls
open Avalonia.Layout
open Avalonia.FuncUI.DSL

type Model = {
  ShowCompletedItems: bool
  TrackerList: TrackerList option
  Services: Services
}

type Msg =
| NoMsg
| ShowCompletedItems
| HideCompletedItems
| ShowOpenFileDialog
| LoadTrackerItems of TrackerList

let mkOptionalMsg f = Option.map f >> Option.defaultValue NoMsg

let init services = 
  let initModel = {
    ShowCompletedItems = false
    TrackerList = None
    Services = services
  }
  initModel, Cmd.none

let private updateUI msg model = 
  match msg with
  | NoMsg -> model
  | ShowCompletedItems -> { model with ShowCompletedItems = true }
  | HideCompletedItems -> { model with ShowCompletedItems = false }
  | LoadTrackerItems list -> { model with TrackerList = Some list }
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
    DockPanel.lastChildFill model.TrackerList.IsSome
    DockPanel.children[
      menu dispatch
      if model.TrackerList.IsSome then
        let trackerList = model.TrackerList.Value
        TextBlock.create [
          TextBlock.dock Dock.Top
          TextBlock.text trackerList.Name
        ]
        CheckBox.create [
          CheckBox.dock Dock.Top
          CheckBox.content "Show all items"
          CheckBox.isChecked model.ShowCompletedItems
          CheckBox.onUnchecked (fun _ -> dispatch HideCompletedItems)
          CheckBox.onChecked (fun _ -> dispatch ShowCompletedItems)
        ]
        ListBox.create [
          ListBox.dataItems trackerList.Items
        ]
    ]
  ]

