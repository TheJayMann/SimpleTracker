open Avalonia
open Avalonia.FuncUI.Components.Hosts
open Avalonia.Themes.Default
open Avalonia.Controls.ApplicationLifetimes
open Elmish
open Avalonia.FuncUI.Elmish
open SimpleTracker
open System.Threading.Tasks

type MainWindow() as this =
  inherit HostWindow()
  do
    base.Title <- "Demo"
    base.Width <- 400.0
    base.Height <- 1000.0

    this.AttachDevTools()

    let getTrackerData () : TrackerItem list Task = [] |> Task.FromResult

    Program.mkProgram Shell.init Shell.update Shell.view
    |> Program.withHost this
    |> Program.runWith { GetTrackerData = getTrackerData }

type App() =
  inherit Application()

  override s.Initialize() =
    DefaultTheme() |> s.Styles.Add

  override s.OnFrameworkInitializationCompleted() =
    match s.ApplicationLifetime with
    | :? IClassicDesktopStyleApplicationLifetime as lifetime -> lifetime.MainWindow <- MainWindow()
    | _ -> ()


[<EntryPoint>]
let main argv =
  AppBuilder
    .Configure<App>()
    .UsePlatformDetect()
    .UseSkia()
    .StartWithClassicDesktopLifetime(argv)