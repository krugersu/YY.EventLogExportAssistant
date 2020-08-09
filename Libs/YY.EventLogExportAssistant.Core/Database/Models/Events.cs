using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class Events : ReferenceObject
    {
        #region Private Static Members

        private static Dictionary<string, string> _mapEventPresentation = new Dictionary<string, string>()
        {
            { "_$Access$_.Access", "Доступ.Доступ" },
            { "_$Access$_.AccessDenied", "Доступ.Отказ в доступе" },
            { "_$Data$_.Delete", "Данные.Удаление" },
            { "_$Data$_.DeletePredefinedData", " Данные.Удаление предопределенных данных" },
            { "_$Data$_.DeleteVersions", "Данные.Удаление версий" },
            { "_$Data$_.New", "Данные.Добавление" },
            { "_$Data$_.NewPredefinedData", "Данные.Добавление предопределенных данных" },
            { "_$Data$_.NewVersion", "Данные.Добавление версиип" },
            { "_$Data$_.Pos", "Данные.Проведение" },
            { "_$Data$_.PredefinedDataInitialization", "Данные.Инициализация предопределенных данных" },
            { "_$Data$_.PredefinedDataInitializationDataNotFound", "Данные.Инициализация предопределенных данных.Данные не найдены" },
            { "_$Data$_.SetPredefinedDataInitialization", "Данные.Установка инициализации предопределенных данных" },
            { "_$Data$_.SetStandardODataInterfaceContent", "Данные.Изменение состава стандартного интерфейса OData" },
            { "_$Data$_.TotalsMaxPeriodUpdate", "Данные.Изменение максимального периода рассчитанных итогов" },
            { "_$Data$_.TotalsMinPeriodUpdate", "Данные.Изменение минимального периода рассчитанных итогов" },
            { "_$Data$_.Post", "Данные.Проведение" },
            { "_$Data$_.Unpost", "Данные.Отмена проведения" },
            { "_$Data$_.Update", "Данные.Изменение" },
            { "_$Data$_.UpdatePredefinedData", "Данные.Изменение предопределенных данных" },
            { "_$Data$_.VersionCommentUpdate", "Данные.Изменение комментария версии" },
            { "_$InfoBase$_.ConfigExtensionUpdate", " Информационная база.Изменение расширения конфигурации" },
            { "_$InfoBase$_.ConfigUpdate", "Информационная база.Изменение конфигурации" },
            { "_$InfoBase$_.DBConfigBackgroundUpdateCancel", "Информационная база.Отмена фонового обновления" },
            { "_$InfoBase$_.DBConfigBackgroundUpdateFinish", "Информационная база.Завершение фонового обновления" },
            { "_$InfoBase$_.DBConfigBackgroundUpdateResume", "Информационная база.Продолжение (после приостановки) процесса фонового обновления" },
            { "_$InfoBase$_.DBConfigBackgroundUpdateStart", "Информационная база.Запуск фонового обновления" },
            { "_$InfoBase$_.DBConfigBackgroundUpdateSuspend", "Информационная база.Приостановка (пауза) процесса фонового обновления" },
            { "_$InfoBase$_.DBConfigExtensionUpdate", "Информационная база.Изменение расширения конфигурации" },
            { "_$InfoBase$_.DBConfigExtensionUpdateError", "Информационная база.Ошибка изменения расширения конфигурации" },
            { "_$InfoBase$_.DBConfigUpdate", "Информационная база.Изменение конфигурации базы данных" },
            { "_$InfoBase$_.DBConfigUpdateStart", "Информационная база.Запуск обновления конфигурации базы данных" },
            { "_$InfoBase$_.DumpError", "Информационная база.Ошибка выгрузки в файл" },
            { "_$InfoBase$_.DumpFinish", "Информационная база.Окончание выгрузки в файл" },
            { "_$InfoBase$_.DumpStart", "Информационная база.Начало выгрузки в файл" },
            { "_$InfoBase$_.EraseData", " Информационная база.Удаление данных информационной баз" },
            { "_$InfoBase$_.EventLogReduce", "Информационная база.Сокращение журнала регистрации" },
            { "_$InfoBase$_.EventLogReduceError", "Информационная база.Ошибка сокращения журнала регистрации" },
            { "_$InfoBase$_.EventLogSettingsUpdate", "Информационная база.Изменение параметров журнала регистрации" },
            { "_$InfoBase$_.EventLogSettingsUpdateError", "Информационная база.Ошибка при изменение настроек журнала регистрации" },
            { "_$InfoBase$_.InfoBaseAdmParamsUpdate", "Информационная база.Изменение параметров информационной базы" },
            { "_$InfoBase$_.InfoBaseAdmParamsUpdateError", "Информационная база.Ошибка изменения параметров информационной базы" },
            { "_$InfoBase$_.IntegrationServiceActiveUpdate", "Информационная база.Изменение активности сервиса интеграции" },
            { "_$InfoBase$_.IntegrationServiceSettingsUpdate", "Информационная база.Изменение настроек сервиса интеграции" },
            { "_$InfoBase$_.MasterNodeUpdate", "Информационная база.Изменение главного узла" },
            { "_$InfoBase$_.PredefinedDataUpdate", "Информационная база.Обновление предопределенных данных" },
            { "_$InfoBase$_.RegionalSettingsUpdate", "Информационная база.Изменение региональных установок" },
            { "_$InfoBase$_.RestoreError", "Информационная база.Ошибка загрузки из файла" },
            { "_$InfoBase$_.RestoreFinish", "Информационная база.Окончание загрузки из файла" },
            { "_$InfoBase$_.RestoreStart", "Информационная база.Начало загрузки из файла" },
            { "_$InfoBase$_.SecondFactorAuthTemplateDelete", "Информационная база.Удаление шаблона вторго фактора аутентификации" },
            { "_$InfoBase$_.SecondFactorAuthTemplateNew", "Информационная база.Добавление шаблона вторго фактора аутентификации" },
            { "_$InfoBase$_.SecondFactorAuthTemplateUpdate", "Информационная база.Изменение шаблона вторго фактора аутентификации" },
            { "_$InfoBase$_.SetPredefinedDataUpdate", "Информационная база.Установить обновление предопределенных данных" },
            { "_$InfoBase$_.TARImportant", "Тестирование и исправление.Ошибка" },
            { "_$InfoBase$_.TARInfo", "Тестирование и исправление.Сообщение" },
            { "_$InfoBase$_.TARMess", "Тестирование и исправление.Предупреждение" },
            { "_$Job$_.Cancel", "Фоновое задание.Отмена" },
            { "_$Job$_.Fail", "Фоновое задание.Ошибка выполнения" },
            { "_$Job$_.Start", "Фоновое задание.Запуск" },
            { "_$Job$_.Succeed", "Фоновое задание.Успешное завершение" },
            { "_$Job$_.Terminate", "Фоновое задание.Принудительное завершение" },
            { "_$OpenIDProvider$_.NegativeAssertion", "Провайдер OpenID.Отклонено" },
            { "_$OpenIDProvider$_.PositiveAssertion", "Провайдер OpenID.Подтверждено" },
            { "_$PerformError$_", "Ошибка выполнения" },
            { "_$Session$_.Authentication", "Сеанс.Аутентификация" },
            { "_$Session$_.AuthenticationError", "Сеанс.Ошибка аутентификации" },
            { "_$Session$_.AuthenticationFirstFactor", "Сеанс.Аутентификация первый фактор" },
            { "_$Session$_.ConfigExtensionApplyError", "Сеанс.Ошибка применения расширения конфигурации" },
            { "_$Session$_.Finish", "Сеанс.Завершение" },
            { "_$Session$_.Start", "Сеанс.Начало" },
            { "_$Transaction$_.Begin", "Транзакция.Начало" },
            { "_$Transaction$_.Commit", "Транзакция.Фиксация" },
            { "_$Transaction$_.Rollback", "Транзакция.Отмена" },
            { "_$User$_.AuthenticationLock", "Пользователи.Блокировка аутентификации" },
            { "_$User$_.AuthenticationUnlock", "Пользователи.Разблокировка аутентификации" },
            { "_$User$_.AuthenticationUnlockError ", "Пользователи.Ошибка разблокировки аутентификации" },
            { "_$User$_.Delete", "Пользователи.Удаление" },
            { "_$User$_.DeleteError", "Пользователи.Ошибка удаления" },
            { "_$User$_.New", "Пользователи.Добавление" },
            { "_$User$_.NewError", "Пользователи.Ошибка добавления" },
            { "_$User$_.Update", "Пользователи.Изменение" },
            { "_$User$_.UpdateError", "Пользователи. Ошибка изменения" }
        };

        #endregion

        #region Public Static Methods

        public static string GetPresentationByName(string name)
        {
            if (_mapEventPresentation.TryGetValue(name, out string output))
                return output;

            return name;
        }

        #endregion

        #region Public Members

        [MaxLength(500)]
        public string Presentation { get; set; }

        #endregion
    }
}
