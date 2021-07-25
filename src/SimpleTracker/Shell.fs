module SimpleTracker.Shell

open Elmish
open Avalonia.Controls.Primitives
open Avalonia.Controls
open Avalonia.Layout
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Components
open Avalonia.Media
open Avalonia
open FSharpPlus.Operators

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
| CompleteItem of id: int
| UndoItem of Id: int

let mkOptionalMsg f = Option.map f >> Option.defaultValue NoMsg

let init services = 
  let initModel = {
    ShowCompletedItems = false
    TaskList = None
    Services = services
  }
  initModel, Cmd.none

let private updateUI msg model = 
  let updateTaskItemStatus itemId isCompleted =
    let updateItem item = { item with IsComplete = isCompleted }
    let rec updateList = function
    | head :: tail when head.Id = itemId -> updateItem head :: tail
    | head :: tail -> head :: updateList tail
    | [] -> []
    let updateList list = { list with Items = updateList list.Items}
    { model with TaskList = model.TaskList |> Option.map updateList }

  match msg with
  | NoMsg -> model
  | ShowCompletedItems -> { model with ShowCompletedItems = true }
  | HideCompletedItems -> { model with ShowCompletedItems = false }
  | LoadTaskList list -> { model with TaskList = Some list }
  | CompleteItem itemId -> updateTaskItemStatus itemId true
  | UndoItem itemId -> updateTaskItemStatus itemId false
  | ShowOpenFileDialog -> model

let private dispatchCmd msg model = 
  match msg with
  | NoMsg -> Cmd.none
  | ShowOpenFileDialog -> 
    LoadTaskList 
    |> mkOptionalMsg 
    |> Cmd.OfTask.perform model.Services.GetTrackerData ()
  | CompleteItem _
  | UndoItem _ -> Cmd.none // TODO: implement auto saving
  | ShowCompletedItems
  | HideCompletedItems 
  | LoadTaskList _ -> Cmd.none

let update msg model = updateUI msg model, dispatchCmd msg model

let private menuView dispatch = 
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

let private taskItemViewTemplate dispatch item =
  StackPanel.create [
    StackPanel.children [
      CheckBox.create [
        CheckBox.isChecked item.IsComplete
        CheckBox.content item.Name
        CheckBox.onChecked (fun _ -> CompleteItem item.Id |> dispatch)
        CheckBox.onUnchecked(fun _ -> UndoItem item.Id |> dispatch)
      ]
      for section in item.RequirementSections do
        TextBlock.create [
          TextBlock.fontWeight FontWeight.Bold
          TextBlock.text $"{section.Name}:"

          Thickness(15., 0., 0., 0.)
          |> TextBlock.margin 
        ]
        for requirement in section.Requirements do
          TextBlock.create [
            TextBlock.text requirement.Name

            Thickness(30., 0., 0., 0.)
            |> TextBlock.margin 
          ]
    ]
  ]

let view model dispatch =
  DockPanel.create [
    DockPanel.verticalAlignment VerticalAlignment.Stretch
    DockPanel.horizontalAlignment HorizontalAlignment.Stretch
    DockPanel.lastChildFill model.TaskList.IsSome
    DockPanel.children[
      menuView dispatch
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
          taskList.Items
          |> Seq.filter (fun i -> model.ShowCompletedItems || not i.IsComplete)
          |> ListBox.dataItems
          
          taskItemViewTemplate dispatch 
          |> DataTemplateView<TaskItem>.create
          |> ListBox.itemTemplate
        ]
    ]
  ]

