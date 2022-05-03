using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZGenesis.Mod;

namespace ZGenesis.Configuration {
    public class ConfigValue {
        private object value;
        private readonly Type type;
        public static Type TypeConvert(string modName, EConfigValueType configValueType) {
            switch(configValueType) {
            case EConfigValueType.Bool:
                return typeof(bool);
            case EConfigValueType.Char:
                return typeof(char);
            case EConfigValueType.String:
                return typeof(string);
            case EConfigValueType.U8:
                return typeof(byte);
            case EConfigValueType.I8:
                return typeof(sbyte);
            case EConfigValueType.U16:
                return typeof(ushort);
            case EConfigValueType.I16:
                return typeof(short);
            case EConfigValueType.U32:
                return typeof(uint);
            case EConfigValueType.I32:
                return typeof(int);
            case EConfigValueType.U64:
                return typeof(ulong);
            case EConfigValueType.I64:
                return typeof(long);
            case EConfigValueType.F32:
                return typeof(float);
            case EConfigValueType.F64:
                return typeof(double);
            case EConfigValueType.F128:
                return typeof(decimal);
            case EConfigValueType.COUNT:
                Logger.Log(Logger.LogLevel.ERROR, modName, "CONFIG ERROR: Invalid  type '{0}'.", Enum.GetName(typeof(EConfigValueType), configValueType));
                return null;
            default:
                Logger.Log(Logger.LogLevel.ERROR, modName, "CONFIG ERROR: Unimplemented type '{0}'.", Enum.GetName(typeof(EConfigValueType),configValueType));
                return null;
            }
        }
        public static ConfigValue TryCreateFromString(string modName, string value, EConfigValueType type) {
            Type t = TypeConvert(modName, type);
            object val = ParseStringValue(modName, value, t);
            if(val == null) {
                Logger.Log(Logger.LogLevel.ERROR, modName, "CONFIG ERROR: Invalid value string (\"{0}\") attempted assignment to invalid type '{1}'.", value, type);
            }
            return new ConfigValue(modName, val, t);
        }
        public ConfigValue(string modName, object value, EConfigValueType type) {
            this.type = TypeConvert(modName, type);
            SetValue(modName, value);
        }
        private ConfigValue(string modName, object value, Type type) {
            this.type = type;
            SetValue(modName, value);
        }
        public object GetValueRaw() {
            return value;
        }
        public void SetValue(string modName, object newValue) {
            if(type.IsAssignableFrom(newValue.GetType()))
                value = newValue;
            else
                Logger.Log(Logger.LogLevel.ERROR, "ZGenesis", "CONFIG ERROR: Invalid value ({0}) attempted assignment to config value with type '{1}'.", newValue, type);
        }
        private static object ParseStringValue(string modName, string value, Type type) {
            TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
            try {
                return typeConverter.ConvertFromString(value);
            } catch(NotSupportedException e) {
                Logger.Log(Logger.LogLevel.ERROR, modName, "Somehow failed to convert (\"{0}\") to type '{1}'. Exception: {2}", value, type, e);
                return null;
            }
        }
        public T GetValue<T>() {
            if(typeof(T).IsAssignableFrom(type)) return (T) value;
            Logger.Log(Logger.LogLevel.ERROR, "ZGenesis", "CONFIG ERROR: Cannot convert value ({0}) to type '{1}'. Expected type: '{2}'.", value, typeof(T), type);
            return default;
        }
    }
}
