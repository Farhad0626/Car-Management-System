using CarApp.Models;
using CarApp.Service;
using CarApp.Exceptions;
using Terminal.Gui;

namespace CarApp.UI;

public class MainWindow : Window
{
    private readonly CarService _service;


    private static readonly ColorScheme AvailableScheme = new ColorScheme
    {
        Normal = Terminal.Gui.Attribute.Make(Color.White, Color.DarkGray),
        Focus = Terminal.Gui.Attribute.Make(Color.White, Color.BrightRed),
        HotNormal = Terminal.Gui.Attribute.Make(Color.BrightYellow, Color.DarkGray),
        HotFocus = Terminal.Gui.Attribute.Make(Color.White, Color.BrightRed),
        Disabled = Terminal.Gui.Attribute.Make(Color.Gray, Color.DarkGray),
    };

    private static readonly ColorScheme SoldScheme = new ColorScheme
    {
        Normal = Terminal.Gui.  Attribute.Make(Color.White, Color.DarkGray),
        Focus = Terminal.Gui.Attribute.Make(Color.White, Color.BrightRed),
        HotNormal = Terminal.Gui.Attribute.Make(Color.BrightYellow, Color.DarkGray),
        HotFocus = Terminal.Gui.Attribute.Make(Color.White, Color.BrightRed),
        Disabled = Terminal.Gui.Attribute.Make(Color.Gray, Color.DarkGray),
    };

    private static readonly ColorScheme AddScheme = new ColorScheme
    {
        Normal = Terminal.Gui.Attribute.Make(Color.White, Color.DarkGray),
        Focus = Terminal.Gui.Attribute.Make(Color.White, Color.BrightRed),
        HotNormal = Terminal.Gui.Attribute.Make(Color.BrightYellow, Color.DarkGray),
        HotFocus = Terminal.Gui.Attribute.Make(Color.White, Color.BrightRed),
        Disabled = Terminal.Gui.Attribute.Make(Color.Gray, Color.DarkGray),
    };

    private static readonly ColorScheme EditScheme = new ColorScheme
    {
        Normal = Terminal.Gui.Attribute.Make(Color.White, Color.Magenta),
        Focus = Terminal.Gui.Attribute.Make(Color.Black, Color.BrightCyan),
        HotNormal = Terminal.Gui.Attribute.Make(Color.BrightYellow, Color.Magenta),
        HotFocus = Terminal.Gui.Attribute.Make(Color.Black, Color.BrightCyan),
        Disabled = Terminal.Gui.Attribute.Make(Color.DarkGray, Color.Magenta),
    };

    public MainWindow(CarService service) : base("Car Management System")
    {
        _service = service;

        var menu = new ListView(new string[]
        {
            "1. Available Cars",
            "2. Sold Cars",
            "3. Add Car",
            "e. Exit"
        })
        {
            Width = Dim.Fill(),
            Height = Dim.Fill() - 1,
            CanFocus = true
        };

        var helpLabel = new Label("[1-3] Select   [E] Exit")
        {
            X = 0,
            Y = Pos.Bottom(menu),
            Width = Dim.Fill()
        };

        menu.OpenSelectedItem += args => RunMenuAction(args.Item);

        KeyPress += args =>
        {
            switch (char.ToLower((char)args.KeyEvent.KeyValue))
            {
                case '1': RunMenuAction(0); args.Handled = true; break;
                case '2': RunMenuAction(1); args.Handled = true; break;
                case '3': RunMenuAction(2); args.Handled = true; break;
                case 'e': RunMenuAction(3); args.Handled = true; break;
            }
        };

        Add(menu, helpLabel);
        menu.SetFocus();
    }

    private void RunMenuAction(int item)
    {
        switch (item)
        {
            case 0: ShowAvailable(); break;
            case 1: ShowSold(); break;
            case 2: ShowAddCar(); break;
            case 3: Application.RequestStop(); break;
        }
    }


    private bool Confirm(string title, string message)
    {
        var result = MessageBox.Query(50, 7, title, message, "_Yes", "_No");
        return result == 0;
    }

    private void Info(string message)
    {
        MessageBox.Query(50, 7, "Success", message, "_OK");
    }


    private void ShowAvailable()
    {
        System.Collections.Generic.List<Car> cars;
        try
        {
            cars = _service.GetAvailableCars().ToList();
        }
        catch (DataAccessException ex)
        {
            MessageBox.ErrorQuery("Database Error", ex.InnerException?.Message ?? ex.Message, "_OK");
            return;
        }

        var items = cars
            .Select(c => $"{c.Id} | {c.Year} {c.Make} {c.Model} | {c.Odometer} mi | {c.Price:C}")
            .ToList();

        if (items.Count == 0) items.Add("(no available cars)");

        var dialog = new Dialog("Available Cars", 90, 22) { ColorScheme = AvailableScheme };

        var listView = new ListView(items)
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill() - 2,
            CanFocus = true
        };

        var helpLabel = new Label("[E]dit  [S]ell  [D]elete  [Esc]Back")
        {
            X = 0,
            Y = Pos.Bottom(listView)
        };

        bool HasSelection(out Car car)
        {
            car = null!;
            var index = listView.SelectedItem;
            if (cars.Count == 0 || index < 0 || index >= cars.Count) return false;
            car = cars[index];
            return true;
        }

        void DoEdit()
        {
            if (!HasSelection(out var car)) return;
            Application.RequestStop();
            ShowEditCar(car);
        }

        void DoSell()
        {
            if (!HasSelection(out var car)) return;
            if (!Confirm("Confirm", $"Mark {car.Year} {car.Make} {car.Model} as sold?")) return;
            try
            {
                _service.MarkCarAsSold(car.Id);
            }
            catch (DataAccessException ex)
            {
                MessageBox.ErrorQuery("Database Error", ex.InnerException?.Message ?? ex.Message, "_OK");
                return;
            }
            Application.RequestStop();
            Info("Car marked as sold!");
            ShowAvailable();
        }

        void DoDelete()
        {
            if (!HasSelection(out var car)) return;
            if (!Confirm("Confirm", $"Delete {car.Year} {car.Make} {car.Model}? This cannot be undone.")) return;
            try
            {
                _service.DeleteCar(car.Id);
            }
            catch (DataAccessException ex)
            {
                MessageBox.ErrorQuery("Database Error", ex.InnerException?.Message ?? ex.Message, "_OK");
                return;
            }
            Application.RequestStop();
            Info("Car deleted!");
            ShowAvailable();
        }

        dialog.KeyPress += args =>
        {
            if (args.KeyEvent.Key == Key.Esc)
            {
                Application.RequestStop();
                args.Handled = true;
                return;
            }

            switch (char.ToLower((char)args.KeyEvent.KeyValue))
            {
                case 'e': DoEdit(); args.Handled = true; break;
                case 's': DoSell(); args.Handled = true; break;
                case 'd': DoDelete(); args.Handled = true; break;
            }
        };

        listView.OpenSelectedItem += _ => DoEdit();

        dialog.Add(listView, helpLabel);
        listView.SetFocus();
        Application.Run(dialog);
    }


    private void ShowSold()
    {
        System.Collections.Generic.List<Car> cars;
        try
        {
            cars = _service.GetSoldCars().ToList();
        }
        catch (DataAccessException ex)
        {
            MessageBox.ErrorQuery("Database Error", ex.InnerException?.Message ?? ex.Message, "_OK");
            return;
        }

        var items = cars
            .Select(c => $"{c.Id} | {c.Year} {c.Make} {c.Model} | {c.Odometer} mi | {c.Price:C}")
            .ToList();

        if (items.Count == 0) items.Add("(no sold cars)");

        var dialog = new Dialog("Sold Cars", 90, 22) { ColorScheme = SoldScheme };

        var listView = new ListView(items)
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill() - 2,
            CanFocus = true
        };

        var helpLabel = new Label("[D]elete  [Esc]Back")
        {
            X = 0,
            Y = Pos.Bottom(listView)
        };

        void DoDelete()
        {
            var index = listView.SelectedItem;
            if (cars.Count == 0 || index < 0 || index >= cars.Count) return;
            var car = cars[index];
            if (!Confirm("Confirm", $"Delete {car.Year} {car.Make} {car.Model}? This cannot be undone.")) return;
            try
            {
                _service.DeleteCar(car.Id);
            }
            catch (DataAccessException ex)
            {
                MessageBox.ErrorQuery("Database Error", ex.InnerException?.Message ?? ex.Message, "_OK");
                return;
            }
            Application.RequestStop();
            Info("Car deleted!");
            ShowSold();
        }

        dialog.KeyPress += args =>
        {
            if (args.KeyEvent.Key == Key.Esc)
            {
                Application.RequestStop();
                args.Handled = true;
                return;
            }

            if (char.ToLower((char)args.KeyEvent.KeyValue) == 'd')
            {
                DoDelete();
                args.Handled = true;
            }
        };

        dialog.Add(listView, helpLabel);
        listView.SetFocus();
        Application.Run(dialog);
    }


    private void ShowAddCar()
    {
        var dialog = new Dialog("Add Car", 60, 18) { ColorScheme = AddScheme };

        var yearField = new TextField("") { X = 15, Y = 1, Width = 40 };
        var makeField = new TextField("") { X = 15, Y = 3, Width = 40 };
        var modelField = new TextField("") { X = 15, Y = 5, Width = 40 };
        var odoField = new TextField("") { X = 15, Y = 7, Width = 40 };
        var priceField = new TextField("") { X = 15, Y = 9, Width = 40 };

        dialog.Add(
            new Label("Year:") { X = 1, Y = 1 }, yearField,
            new Label("Make:") { X = 1, Y = 3 }, makeField,
            new Label("Model:") { X = 1, Y = 5 }, modelField,
            new Label("Odometer:") { X = 1, Y = 7 }, odoField,
            new Label("Price:") { X = 1, Y = 9 }, priceField
        );

        var helpLabel = new Label("[Enter] Save   [Esc] Cancel") { X = 1, Y = 11 };
        dialog.Add(helpLabel);

        void DoSave()
        {
            if (!int.TryParse(yearField.Text.ToString(), out var year))
            {
                MessageBox.ErrorQuery("Invalid input", "Year must be a whole number.", "_OK");
                return;
            }
            if (!long.TryParse(odoField.Text.ToString(), out var odometer))
            {
                MessageBox.ErrorQuery("Invalid input", "Odometer must be a whole number.", "_OK");
                return;
            }
            if (!decimal.TryParse(priceField.Text.ToString(), out var price))
            {
                MessageBox.ErrorQuery("Invalid input", "Price must be a number.", "_OK");
                return;
            }

            var make = makeField.Text.ToString() ?? "";
            var model = modelField.Text.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(make) || string.IsNullOrWhiteSpace(model))
            {
                MessageBox.ErrorQuery("Invalid input", "Make and Model are required.", "_OK");
                return;
            }

            try
            {
                _service.AddCar(year, make, model, odometer, price);
            }
            catch (DataAccessException ex)
            {
                MessageBox.ErrorQuery("Database Error", ex.InnerException?.Message ?? ex.Message, "_OK");
                return;
            }
            Application.RequestStop();
            Info("Car added!");
        }

        dialog.KeyPress += args =>
        {
            if (args.KeyEvent.Key == Key.Esc)
            {
                Application.RequestStop();
                args.Handled = true;
            }
            else if (args.KeyEvent.Key == Key.Enter)
            {
                DoSave();
                args.Handled = true;
            }
        };

        yearField.SetFocus();
        Application.Run(dialog);
    }


    private void ShowEditCar(Car car)
    {
        var dialog = new Dialog("Edit Car", 60, 18) { ColorScheme = EditScheme };

        var yearField = new TextField(car.Year.ToString()) { X = 15, Y = 1, Width = 40 };
        var makeField = new TextField(car.Make) { X = 15, Y = 3, Width = 40 };
        var modelField = new TextField(car.Model) { X = 15, Y = 5, Width = 40 };
        var odoField = new TextField(car.Odometer.ToString()) { X = 15, Y = 7, Width = 40 };
        var priceField = new TextField(car.Price.ToString()) { X = 15, Y = 9, Width = 40 };

        dialog.Add(
            new Label("Year:") { X = 1, Y = 1 }, yearField,
            new Label("Make:") { X = 1, Y = 3 }, makeField,
            new Label("Model:") { X = 1, Y = 5 }, modelField,
            new Label("Odometer:") { X = 1, Y = 7 }, odoField,
            new Label("Price:") { X = 1, Y = 9 }, priceField
        );

        var helpLabel = new Label("[Enter] Save   [Esc] Cancel") { X = 1, Y = 11 };
        dialog.Add(helpLabel);

        void DoSave()
        {
            if (!int.TryParse(yearField.Text.ToString(), out var year))
            {
                MessageBox.ErrorQuery("Invalid input", "Year must be a whole number.", "_OK");
                return;
            }
            if (!long.TryParse(odoField.Text.ToString(), out var odometer))
            {
                MessageBox.ErrorQuery("Invalid input", "Odometer must be a whole number.", "_OK");
                return;
            }
            if (!decimal.TryParse(priceField.Text.ToString(), out var price))
            {
                MessageBox.ErrorQuery("Invalid input", "Price must be a number.", "_OK");
                return;
            }

            var make = makeField.Text.ToString() ?? "";
            var model = modelField.Text.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(make) || string.IsNullOrWhiteSpace(model))
            {
                MessageBox.ErrorQuery("Invalid input", "Make and Model are required.", "_OK");
                return;
            }

            try
            {
                _service.UpdateCar(car, year, make, model, odometer, price);
            }
            catch (DataAccessException ex)
            {
                MessageBox.ErrorQuery("Database Error", ex.InnerException?.Message ?? ex.Message, "_OK");
                return;
            }
            Application.RequestStop();
            Info("Car updated!");
        }

        dialog.KeyPress += args =>
        {
            if (args.KeyEvent.Key == Key.Esc)
            {
                Application.RequestStop();
                args.Handled = true;
            }
            else if (args.KeyEvent.Key == Key.Enter)
            {
                DoSave();
                args.Handled = true;
            }
        };

        yearField.SetFocus();
        Application.Run(dialog);
    }
}