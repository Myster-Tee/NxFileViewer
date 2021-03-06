using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;

namespace Emignatik.NxFileViewer.Utils.MVVM.Localization
{
    public class LocalizationManager<TKeys> : ILocalizationManager<TKeys> where TKeys : ILocalizationKeysBase
    {
        private readonly string _defaultSystemCultureName = Thread.CurrentThread.CurrentUICulture.Name;

        private ILocalization<TKeys> _current;
        private readonly RealLocalization<TKeys>[] _realLocalizations;

        private AutoLocalization<TKeys>? _autoLocalization;
        private readonly ILocalization<TKeys> _fallbackLocalization;
        private readonly RealLocalization<TKeys>? _systemLocalization;
        private bool _useAutoLocalization;

        public event EventHandler<LocalizationChangedHandlerArgs<TKeys>>? LocalizationChanged;
        public event PropertyChangedEventHandler? PropertyChanged;


        public LocalizationManager(bool useAutoLocalization = true) : this(new[] { Assembly.GetEntryAssembly(), Assembly.GetExecutingAssembly() }, useAutoLocalization)
        {
        }

        public LocalizationManager(IEnumerable<Assembly> assemblies, bool useAutoLocalization = true) : this(FindAvailableLocalizations(assemblies), useAutoLocalization)
        {
        }

        public LocalizationManager(IEnumerable<RealLocalization<TKeys>> realLocalizations, bool useAutoLocalization = true)
        {
            _realLocalizations = realLocalizations.ToArray();
            if (_realLocalizations.Length <= 0)
                throw new ArgumentException($"At least one localization should be specified.", nameof(realLocalizations));

            _fallbackLocalization = _realLocalizations.FirstOrDefault(loc => loc.Keys.IsFallback) ?? _realLocalizations.First();

            _systemLocalization = _realLocalizations.FirstOrDefault(localization => localization.CultureName == _defaultSystemCultureName);

            _useAutoLocalization = useAutoLocalization;
            InitializeAutoLocalization();

            _current = AvailableLocalizations.First();
        }

        private static List<RealLocalization<TKeys>> FindAvailableLocalizations(IEnumerable<Assembly> assemblies)
        {
            var realLocalizations = new List<RealLocalization<TKeys>>();

            var keysType = typeof(TKeys);

            var typesSet = new HashSet<Type>();
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (keysType.IsAssignableFrom(type) && type != keysType && type.IsClass && !typesSet.Contains(type))
                        typesSet.Add(type);
                }
            }

            foreach (var type in typesSet)
            {
                var constructorInfo = type.GetConstructor(new Type[0]);

                if (constructorInfo == null)
                {
                    Trace.TraceWarning($"Type {keysType.FullName} seems to be a Localization, but parameterless constructor is missing.");
                    continue;
                }

                var localizationKeys = (TKeys)constructorInfo.Invoke(new object?[0]);
                var realLocalization = new RealLocalization<TKeys>(localizationKeys);

                realLocalizations.Add(realLocalization);
            }

            if (realLocalizations.Count <= 0)
                throw new Exception($"No localization found: no loaded implementation of type {keysType.Name} found in the specified assemblies.");

            return realLocalizations;
        }

        public IEnumerable<ILocalization<TKeys>> AvailableLocalizations
        {
            get
            {
                if (_autoLocalization != null)
                    yield return _autoLocalization;

                foreach (var localization in _realLocalizations)
                    yield return localization;
            }
        }

        public ILocalization<TKeys> FallbackLocalization => _fallbackLocalization;

        public ILocalization<TKeys>? SystemLocalization => _systemLocalization;

        public ILocalization<TKeys> Current
        {
            get => _current;
            set
            {
                if (value == _current)
                    return;

                _current = value ?? throw new ArgumentNullException(nameof(Current));

                if (_autoLocalization != null)
                {
                    if (_autoLocalization == _current)
                        _autoLocalization.DisplayName = _autoLocalization.Keys.LanguageAuto;
                    else
                        _autoLocalization.DisplayName = _current.Keys.LanguageAuto;
                }

                OnLocalizationChanged();
                NotifyPropertyChanged();
                NotifyLocalizationChanged(_current);
            }
        }

        public bool UseAutoLocalization
        {
            get => _useAutoLocalization;
            set
            {
                _useAutoLocalization = value;
                InitializeAutoLocalization();
            }
        }


        public void AddWeakLocalizationChangedHandler(EventHandler<LocalizationChangedHandlerArgs<TKeys>> localizationChangedHandler)
        {
            WeakEventManager<LocalizationManager<TKeys>, LocalizationChangedHandlerArgs<TKeys>>.AddHandler(this, nameof(LocalizationChanged), localizationChangedHandler);
        }

        public void RemoveWeakLocalizationChangedHandler(EventHandler<LocalizationChangedHandlerArgs<TKeys>> localizationChangedHandler)
        {
            WeakEventManager<LocalizationManager<TKeys>, LocalizationChangedHandlerArgs<TKeys>>.RemoveHandler(this, nameof(LocalizationChanged), localizationChangedHandler);
        }

        private void InitializeAutoLocalization()
        {
            if (UseAutoLocalization)
            {
                var keys = _systemLocalization != null ? _systemLocalization.Keys : _fallbackLocalization.Keys;
                _autoLocalization = new AutoLocalization<TKeys>(keys);
            }
            else
            {
                _autoLocalization = null;
            }

            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(AvailableLocalizations));
        }

        protected virtual void OnLocalizationChanged()
        {
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void NotifyLocalizationChanged(ILocalization<TKeys> localization)
        {
            LocalizationChanged?.Invoke(this, new LocalizationChangedHandlerArgs<TKeys>(localization));
        }
    }

}