# Recipe Management App
### Object-Oriented Design — Year 2 Project | Atlantic Technological University

A full-featured recipe management application built in C# as part of an Object-Oriented Design module. 
The repository contains two versions of the app, representing the progression of the project over the semester.

---

## Repository Structure

```
RecipeApp.WPF/    — Version 1: WPF (.NET Framework 4.7.2)
RecipeApp.MAUI/   — Version 2: .NET MAUI (current, final version)
```

### Why Two Versions?

The project began as a WPF application. After completing the WPF version, the decision was made to rebuild it from scratch in .NET MAUI with a proper MVVM architecture, improved ID naming conventions, and a shared class library. The WPF version is kept in the repository to show the progression and development of the project over the semester.

---

## Final Version — .NET MAUI

The MAUI version is the current and final version of the app. It is a fully featured recipe management application targeting Windows desktop.

### Features

- **Browse Recipes** — Fetches recipes from the DummyJSON API with multi-filter search (name, ingredient, cuisine, difficulty, protein, max time)
- **Recipe Detail** — Full ingredients list and numbered step-by-step instructions
- **Favourites** — Save and manage favourite recipes with persistent local storage
- **Add Recipe** — Create and save custom recipes with full validation
- **Meal Planner** — Weekly calendar to plan meals by day and meal type, with week navigation
- **Shopping List** — Auto-generated from the meal plan with checkbox items and copy to clipboard

### Tech Stack

| Layer | Technology |
|---|---|
| UI Framework | .NET MAUI |
| Architecture | MVVM (CommunityToolkit.Mvvm) |
| Database | SQLite (sqlite-net-pcl) |
| API | DummyJSON Recipes API |
| JSON | Newtonsoft.Json |
| Shared Logic | RecipeApp.Shared (class library) |
| Tests | MSTest (RecipeApp.Tests) |

---

## Project Structure — MAUI Version

```
RecipeApp.MAUI/
  RecipeApp.MAUI/
    Views/            — XAML pages (6 pages)
    ViewModels/       — MVVM ViewModels for each page
    Services/         — DatabaseService, RecipeApiService
    Helpers/          — Converters.cs (IValueConverter implementations)
    Resources/
      Styles/         — Colors.xaml, Styles.xaml (named styles and brand colours)
      Images/         — SVG icons and fallback image

RecipeApp.Shared/     — Shared class library
  Recipe.cs
  MealPlan.cs
  RecipeResponse.cs
  RecipeHelper.cs     — Testable pure logic (sorting, validation, date helpers)

RecipeApp.Tests/      — MSTest unit test project (18 tests)
  RecipeHelperTests.cs
  RecipeModelTests.cs
```

---

## Requirements Coverage

| Requirement | Implementation |
|---|---|
| WPF/MAUI + XAML | .NET MAUI with hand-coded XAML throughout |
| Classes/Objects | Recipe, MealPlan, RecipeResponse, DayMeals, ShoppingItem |
| Inheritance | All ViewModels inherit from BaseViewModel |
| Interfaces | IRecipeApiService — decouples ViewModels from API implementation |
| Sorting/Filter/Searching | Multi-filter LINQ panel in RecipesViewModel |
| Lists/Observable Collections | ObservableCollection used in all ViewModels |
| Event Handling | Commands (RelayCommand), button clicks, OnAppearing |
| Working with Dates | Week navigation, date filtering, DateTime.Today |
| GitHub | Two branches — main (WPF), maui-version (MAUI) |
| Hand-coded XAML | No drag-and-drop designer used |
| Database | SQLite with sqlite-net-pcl, two tables (Recipes, MealPlans) |
| LINQ | Where, OrderBy, GroupBy, Select, Any, Distinct throughout |
| Additional Windows/Navigation | Shell flyout + GoToAsync routing to RecipeDetailPage |
| JSON | DummyJSON API parsing, JSON string storage for lists |
| Images | Recipe images from API, fallback no_image.png, SVG icons |
| Styles | Named styles and brand colours defined in Colors.xaml and Styles.xaml |
| Data Templates | CollectionView item templates on all list pages |
| Exception Handling | Try/catch on all async operations, user-facing error alerts |
| Testing | 18 unit tests across RecipeHelperTests and RecipeModelTests |
| Something Extra | Custom IValueConverter implementations (9 converters in Converters.cs) and thread-safe async database initialisation using SemaphoreSlim |

---

## How to Run

### Prerequisites
- Visual Studio 2022 (v17.8 or later)
- .NET 9 SDK
- .NET MAUI workload installed (`dotnet workload install maui`)

### Steps
1. Clone the repository
2. Open `RecipeApp.MAUI/RecipeApp.MAUI.sln` in Visual Studio
3. Set `RecipeApp.MAUI` as the startup project
4. Select **Windows Machine** as the target
5. Press **F5** to build and run

### Running Tests
1. Open the solution in Visual Studio
2. Go to **Test** → **Run All Tests**
3. All 18 tests should pass in Test Explorer

---

## WPF Version

The original WPF version (`RecipeApp.WPF/`) is a fully working application built on .NET Framework 4.7.2 with Entity Framework 6 and LocalDB. It implements all the same core features but without MVVM architecture. It is kept on the `main` branch as a record of the earlier stage of the project.

To run the WPF version, open `RecipeApp.WPF/RecipeApp.sln` and run the `RecipeApp` project. LocalDB must be installed (included with Visual Studio).

---

## Author

**Adriana Prepeshniuk**  
