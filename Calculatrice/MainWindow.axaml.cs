using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


//ceci est une calculatrice
namespace Calculatrice
{
    /// <summary>
    /// Fenêtre principale de l'application Calculatrice.
    /// Implémente une calculatrice basique avec les opérations arithmétiques standards.
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Champs privés pour stocker l'état de la calculatrice
        private string _displayText = "0";           // Texte affiché sur l'écran
        private double _firstNumber = 0;             // Premier nombre de l'opération
        private string _operation = "";              // Opération en cours (+, -, ×, ÷)
        private bool _newNumber = true;              // Indique si le prochain chiffre commence un nouveau nombre

        // Implémentation de INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notifie les observateurs qu'une propriété a changé.
        /// </summary>
        /// <param name="propertyName">Nom de la propriété modifiée.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Propriété pour le texte affiché sur l'écran de la calculatrice.
        /// Notifie l'interface utilisateur des changements via INotifyPropertyChanged.
        /// </summary>
        public string DisplayText
        {
            get => _displayText;
            set
            {
                if (_displayText != value)
                {
                    _displayText = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Constructeur de la fenêtre principale.
        /// Initialise les composants et configure les gestionnaires d'événements.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Attache les gestionnaires de clic aux boutons après le chargement de l'interface
            Loaded += (s, e) =>
            {
                var buttonGrid = this.FindControl<Grid>("ButtonGrid");
                if (buttonGrid != null)
                {
                    foreach (Button button in buttonGrid.Children)
                    {
                        button.Click += Button_Click;
                    }
                }
            };
        }

        /// <summary>
        /// Gestionnaire d'événements principal pour tous les boutons de la calculatrice.
        /// Redirige vers la méthode appropriée selon le bouton cliqué.
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var value = button.Content.ToString();

            // Routage des actions selon le bouton pressé
            switch (value)
            {
                case "AC":
                    Clear();
                    break;
                case "+/-":
                    Negate();
                    break;
                case "%":
                    Percentage();
                    break;
                case "=":
                    Calculate();
                    break;
                case "+":
                case "-":
                case "×":
                case "÷":
                    SetOperation(value);
                    break;
                case ".":
                    AddDecimalPoint();
                    break;
                default:
                    AddNumber(value);
                    break;
            }
        }

        /// <summary>
        /// Réinitialise la calculatrice à son état initial.
        /// </summary>
        private void Clear()
        {
            DisplayText = "0";
            _firstNumber = 0;
            _operation = "";
            _newNumber = true;
        }

        /// <summary>
        /// Inverse le signe du nombre actuellement affiché.
        /// </summary>
        private void Negate()
        {
            // Conversion avec gestion explicite de la culture pour éviter les erreurs de parsing
            var currentValue = double.Parse(DisplayText.Replace(',', '.'), 
                System.Globalization.CultureInfo.InvariantCulture);
            DisplayText = (-currentValue).ToString();
        }

        /// <summary>
        /// Convertit le nombre affiché en pourcentage (divise par 100).
        /// </summary>
        private void Percentage()
        {
            var currentValue = double.Parse(DisplayText);
            DisplayText = (currentValue / 100).ToString();
        }

        /// <summary>
        /// Enregistre l'opération à effectuer et prépare la saisie du deuxième nombre.
        /// </summary>
        /// <param name="operation">Opérateur mathématique à appliquer.</param>
        private void SetOperation(string operation)
        {
            // Parse le premier nombre avec gestion de la culture
            _firstNumber = double.Parse(DisplayText.Replace(',', '.'), 
                System.Globalization.CultureInfo.InvariantCulture);
            _operation = operation;
            _newNumber = true;
        }

        /// <summary>
        /// Ajoute un point décimal au nombre si celui-ci n'en contient pas déjà un.
        /// </summary>
        private void AddDecimalPoint()
        {
            if (!DisplayText.Contains("."))
            {
                DisplayText += ".";
            }
        }

        /// <summary>
        /// Ajoute un chiffre au nombre actuellement affiché.
        /// </summary>
        /// <param name="number">Chiffre à ajouter.</param>
        private void AddNumber(string number)
        {
            if (_newNumber)
            {
                DisplayText = number;
                _newNumber = false;
            }
            else
            {
                // Remplace le 0 initial ou concatène le nouveau chiffre
                DisplayText = DisplayText == "0" ? number : DisplayText + number;
            }
        }

        /// <summary>
        /// Effectue le calcul avec l'opération en cours et affiche le résultat.
        /// Gère les cas d'erreur comme la division par zéro.
        /// </summary>
        private void Calculate()
        {
            try
            {
                // Parse le second nombre avec gestion de la culture
                var secondNumber = double.Parse(DisplayText.Replace(',', '.'), 
                    System.Globalization.CultureInfo.InvariantCulture);
                double result = 0;

                // Effectue l'opération appropriée
                switch (_operation)
                {
                    case "+":
                        result = _firstNumber + secondNumber;
                        break;
                    case "-":
                        result = _firstNumber - secondNumber;
                        break;
                    case "×":
                        result = _firstNumber * secondNumber;
                        break;
                    case "÷":
                        // Vérifie la division par zéro
                        if (secondNumber != 0)
                            result = _firstNumber / secondNumber;
                        else
                        {
                            DisplayText = "Error";
                            return;
                        }
                        break;
                }

                // Affiche le résultat et réinitialise pour la prochaine opération
                DisplayText = result.ToString();
                _operation = "";
                _newNumber = true;
            }
            catch (Exception)
            {
                DisplayText = "Error";
            }
        }
    }
}