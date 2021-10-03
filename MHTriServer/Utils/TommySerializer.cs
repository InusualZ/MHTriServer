﻿// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/Tommy.Serializer              --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------
// Heavily Modified (09/19/2021) by Wesley Moret (InusualZ) https://github.com/InusualZ

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Tommy.Serializer
{
    /// <summary>
    /// A class to enable (De)Serialization of a class instance to/from disk
    /// </summary>
    public static class TommySerializer
    {
        /// <summary>
        /// Basic Tommy Serializer (Specific) Exception
        /// </summary>
        public class TommySerializerException : Exception 
        { 
            public TommySerializerException(string reason) : base(reason) {}
            public TommySerializerException(string reason, Exception inner) : base(reason, inner) { }
        }

        /// <summary>
        /// Reflectively determines the property types and values of the passed class instance and outputs a Toml file
        /// </summary>
        /// <param name="data">The class instance in which the properties will be used to create a Toml file </param>
        /// <param name="path">The destination path in which to create the Toml file</param>
        /// <param name="writeToMemory">Write to <see cref="System.IO.MemoryStream"/>. If omitted, a standard <see cref="System.IO.StreamWriter"/> will be used. </param>
        public static MemoryStream ToTomlFile(object data, string path, bool writeToMemory = false) =>
            ToTomlFile(new[] { data }, path, writeToMemory);

        /// <summary>
        /// Reflectively determines the property types and values of the passed class instance and outputs a Toml file
        /// </summary>
        /// <param name="datas">The class instances in which the properties will be used to create a Toml file </param>
        /// <param name="path">The destination path in which to create the Toml file</param>
        /// <param name="writeToMemory">Write to <see cref="System.IO.MemoryStream"/>. If omitted, a standard <see cref="System.IO.StreamWriter"/> will be used. </param>
        public static MemoryStream ToTomlFile(object[] datas, string path, bool writeToMemory = false)
        {
            if (datas == null || datas.Length == 0)
            {
                throw new ArgumentNullException("Error: object parameters are null.");
            }

            TomlTable tomlTable = new TomlTable();
            bool isMultiObject = datas.Length > 1;

            for (int t = 0; t < datas.Length; t++)
            {
                object data = datas[t];
                try
                {
                    List<SortNode> tomlData = new List<SortNode>();
                    TomlTable tomlDataTable = new TomlTable();
                    Type type = data.GetType();

                    // -- Check object for table name attribute --------------------
                    string tableName = type.GetCustomAttribute<TommyTableName>()?.TableName;

                    // -- Iterate the properties of the object ---------------------
                    PropertyInfo[] properties = type.GetProperties(bindingFlags);
                    ProcessPropertiesToFile(data, properties, ref tomlData);

                    // -- Iterate the fields of the object -------------------------
                    FieldInfo[] fields = type.GetFields(bindingFlags);
                    ProcessFieldsToFile(data, fields, ref tomlData);

                    // -- Check if sorting needs to be done to properties. ---------
                    // -- Properties that do not have a sort attribute are ---------
                    // -- given a sort order of the max sort int +1 and ------------
                    // -- appear after the sorted properties -----------------------
                    int maxSortInt = (from l in tomlData select l.SortOrder).Max();
                    if (maxSortInt > -1) tomlData = tomlData.SortNodes(maxSortInt);

                    tomlData.ForEach(n => { tomlDataTable[n.Name] = n.Value; });

                    if (!string.IsNullOrEmpty(tableName)) tomlTable[tableName] = tomlDataTable;
                    else
                    {
                        if (isMultiObject) tomlTable[type.Name] = tomlDataTable;
                        tomlTable.Add(tomlDataTable);
                    }
                }
                catch (Exception e)
                {
                    throw new TommySerializerException(e.Message, e);
                }
            }

            if (writeToMemory) return WriteToMemory(tomlTable);
            else WriteToDisk(tomlTable, path);
            return null;
        }

        /// <summary>
        /// Creates a new instance of Type <typeparamref name="T"/> and parses Toml file from <paramref name="path"/>
        /// </summary>
        /// <param name="path">The full path to the existing Toml file you wish to parse</param>
        /// <param name="memoryStream">Instead of reading from a file, a MemoryStream can be used.</param>
        /// <typeparam name="T">The Type of class in which the parsed Toml data will be assigned</typeparam>
        /// <returns>An instantiated class of Type <typeparamref name="T"/></returns>
        public static T FromTomlFile<T>(string path, MemoryStream memoryStream = null) where T : class, new()
        {
            try
            {
                // -- Create new instance of class Type T ----------------
                T dataClass = Activator.CreateInstance<T>();

                // -- Parse Toml data from either memory or disk ---------
                TomlTable table = memoryStream != null ? ReadFromMemory(memoryStream) : ReadFromDisk(path);

                Type fileType = typeof(T);

                if (fileType.GetCustomAttribute<TommyRoot>() != null)
                {
                    List<string> tableKeys = table.Keys.ToList();

                    // -- Process Properties -------------------------------------------
                    PropertyInfo[] properties = fileType.GetProperties(bindingFlags);
                    ProcessPropertiesFromFile(table, tableKeys, properties, dataClass);

                    // -- Process Fields -----------------------------------------------
                    FieldInfo[] fields = fileType.GetFields(bindingFlags)
                        .Where(x => !x.Name.Contains("k__BackingField"))
                        .ToArray();

                    ProcessFieldsFromFile(table, tableKeys, fields, dataClass);
                }
                else
                {
                    string tableName = fileType.GetCustomAttribute<TommyTableName>()?.TableName ?? fileType.Name;

                    TomlNode tableData = table[tableName];
                    List<string> tableKeys = tableData.Keys.ToList();

                    // -- Process Properties -------------------------------------------
                    PropertyInfo[] properties = fileType.GetProperties(bindingFlags);
                    ProcessPropertiesFromFile(tableData, tableKeys, properties, dataClass);

                    // -- Process Fields -----------------------------------------------
                    FieldInfo[] fields = fileType.GetFields(bindingFlags)
                        .Where(x => !x.Name.Contains("k__BackingField") && tableKeys.Contains(x.Name))
                        .ToArray();

                    ProcessFieldsFromFile(tableData, tableKeys, fields, dataClass);
                }
                return dataClass;
            }
            catch (Exception e)
            {
                throw new TommySerializerException(e.Message, e);
            }
        }

        #region Process Properties

        internal static void ProcessPropertiesToFile(object data, PropertyInfo[] properties, ref List<SortNode> tomlData)
        {
            foreach (PropertyInfo property in properties)
            {
                // -- Check for included/ignored properties ----------------
                if (Attribute.IsDefined(property, typeof(TommyIgnore))) continue;
                if (!property.PropertyType.IsPublic && !Attribute.IsDefined(property, typeof(TommyInclude))) continue;

                // -- Check if property has comment attribute --------------
                string comment = (Attribute.GetCustomAttribute(property, typeof(TommyComment), false) as TommyComment)?.Value;

                // -- Check if property has SortOrder attribute ------------
                var valueAttribute = Attribute.GetCustomAttribute(property, typeof(TommyValue), false) as TommyValue;
                int? sortOrder = valueAttribute?.SortOrder;

                var propertyValue = data.GetPropertyValue(property.Name);
                Type propertyType = property.PropertyType;

                // -- Check each property type in order to
                // -- determine which type of TomlNode to create
                if (propertyType == typeof(string) || (propertyType.GetInterface(nameof(IEnumerable)) == null && !propertyType.IsArray))
                {
                    TomlNode valueNode = GetTomlNode(propertyValue, propertyType);
                    valueNode.Comment = comment;

                    tomlData.Add(new SortNode { Name = property.Name, SortOrder = sortOrder ?? -1, Value = valueNode });
                }

                else if (propertyType.GetInterface(nameof(IEnumerable)) != null && propertyType.GetInterface(nameof(IDictionary)) != null)
                {
                    IDictionary typeValue = propertyValue as IDictionary;
                    if (typeValue == null) continue;

                    Type[] dictTypeArguments = propertyType.IsGenericType && !propertyType.IsGenericTypeDefinition
                        ? propertyType.GetGenericArguments()
                        : Type.EmptyTypes;

                    // var dictTypeArguments = typeValue.GetType().GenericTypeArguments;
                    Type kType = dictTypeArguments[0];
                    Type vType = dictTypeArguments[1];

                    TomlNode dictionaryNode = CreateTomlDictionary(kType, vType, typeValue, propertyType);
                    dictionaryNode.Comment = comment;

                    tomlData.Add(new SortNode { Name = property.Name, SortOrder = sortOrder ?? -1, Value = dictionaryNode });
                }
                else
                {
                    IList propAsList = propertyValue as IList;
                    TomlArray tomlArray = new TomlArray { Comment = comment }; // @formatter:off
                    Type propArgType = propertyType.GetElementType() ?? propertyType.GetGenericArguments().FirstOrDefault();

                    if (propAsList == null) 
                    { 
                        throw new TommySerializerException($"{property.Name} could not be cast as IList."); 
                    }  // @formatter:on

                    for (int i = 0; i < propAsList.Count; i++)
                    {
                        if (propAsList[i] == null) throw new TommySerializerException($"collection value cannot be null");
                        tomlArray.Add(GetTomlNode(propAsList[i], propArgType));
                    }

                    tomlData.Add(new SortNode { Name = property.Name, SortOrder = sortOrder ?? -1, Value = tomlArray });
                }
            }
        }

        internal static void ProcessPropertiesFromFile(TomlNode tableData, List<string> tableKeys, PropertyInfo[] properties, object dataClass)
        {
            for (int k = 0; k < tableKeys.Count; k++)
            {
                string key = tableKeys[k];
                var property = properties.FirstOrDefault((prop) => {
                    var altName = prop.GetCustomAttribute<TommyValue>()?.Name;
                    return altName == key || prop.Name == key;
                });

                if (property == null) continue;

                if (property.GetCustomAttribute<TommyIgnore>() != null) continue;

                Type propertyType = property.PropertyType;

                TomlNode node = tableData[key];
                if (!node.HasValue && !node.IsTable) continue;

                // -- Determine if property is not a collection or table -------
                if (propertyType == typeof(string) && tableData[key].IsString || !tableData[key].IsArray && !tableData[key].IsTable)
                {
                    dataClass.SetPropertyValue(key, GetValueByType(tableData[key], propertyType));
                }
                // -- Determine if property is a Toml Table/IDictionary --------
                else if (node.IsTable)
                {
                    if (propertyType.GetInterface(nameof(IEnumerable)) != null && propertyType.GetInterface(nameof(IDictionary)) != null)
                    {
                        dataClass.SetPropertyValue(key, CreateGenericDictionary(tableData[key], propertyType));
                        continue;
                    }

                    if (!propertyType.IsClass)
                    {
                        throw new TommySerializerException($"Could not deserialize TomlTable into `{property.Name}` of type {propertyType.Name}");
                    }

                    var propertyDataClass = Activator.CreateInstance(propertyType) ?? throw new TommySerializerException($"Unable to instantiate {propertyType.Name}");
                    property.SetValue(dataClass, propertyDataClass);

                    List<string> propertyTableKeys = node.Keys.ToList();

                    // -- Process Properties -------------------------------------------
                    PropertyInfo[] dataClassProperties = propertyType.GetProperties(bindingFlags);

                    ProcessPropertiesFromFile(node, propertyTableKeys, dataClassProperties, propertyDataClass);

                    // -- Process Fields -----------------------------------------------
                    FieldInfo[] dataClassFields = propertyType.GetFields(bindingFlags)
                        .Where(x => !x.Name.Contains("k__BackingField"))
                        .ToArray();

                    ProcessFieldsFromFile(node, propertyTableKeys, dataClassFields, propertyDataClass);

                }
                else
                {
                    if (propertyType.GetInterface(nameof(IEnumerable)) == null) continue;

                    Type valueType = propertyType.GetElementType() ?? propertyType.GetGenericArguments().FirstOrDefault();
                    if (valueType == null)
                    {
                        throw new TommySerializerException($"Could not find argument type for property: {propertyType.Name}.");
                    }

                    TomlNode[] array = node.AsArray.RawArray.ToArray();

                    if (valueType.GetInterface(nameof(IConvertible)) != null)
                        dataClass.SetPropertyValue(key, CreateGenericList(array, propertyType));
                    else if (valueType.IsClass)
                    {
                        var propertyValue = Array.CreateInstance(valueType, array.Length);

                        for(var elementIndex = 0; elementIndex < propertyValue.Length; ++elementIndex)
                        {
                            var element = array[elementIndex];
                            var valueDataClass = Activator.CreateInstance(valueType) ?? throw new TommySerializerException($"Unable to instantiate {propertyType.Name}");
                            propertyValue.SetValue(valueDataClass, elementIndex);

                            List<string> propertyTableKeys = element.Keys.ToList();

                            // -- Process Properties -------------------------------------------
                            PropertyInfo[] dataClassProperties = valueType.GetProperties(bindingFlags);

                            ProcessPropertiesFromFile(element, propertyTableKeys, dataClassProperties, valueDataClass);

                            // -- Process Fields -----------------------------------------------
                            FieldInfo[] dataClassFields = propertyType.GetFields(bindingFlags)
                                .Where(x => !x.Name.Contains("k__BackingField"))
                                .ToArray();

                            ProcessFieldsFromFile(element, propertyTableKeys, dataClassFields, valueDataClass);
                        }

                        property.SetValue(dataClass, propertyValue);
                    }
                    else throw new TommySerializerException($"{valueType.Name} is not able to be converted.");
                }
            }
        }

        #endregion

        #region Process Fields

        internal static void ProcessFieldsToFile(object data, FieldInfo[] fields, ref List<SortNode> tomlData)
        {
            foreach (FieldInfo field in fields)
            {
                // -- Check for included/ignored fields ------------------
                if (Attribute.IsDefined(field, typeof(TommyIgnore))) continue;
                if (!field.FieldType.IsPublic && !Attribute.IsDefined(field, typeof(TommyInclude)) || field.Name.Contains("k__BackingField")) continue;

                // -- Check if field has comment attribute ---------------
                string comment = (Attribute.GetCustomAttributes(field, typeof(TommyComment), false).FirstOrDefault() as TommyComment)?.Value;

                // -- Check if property has SortOrder attribute ----------
                
                var valueAttribute = field.GetCustomAttribute<TommyValue>();
                int? sortOrder = valueAttribute?.SortOrder;

                var fieldValue = data.GetFieldValue(field.Name);
                Type fieldType = field.FieldType;

                // -- Check each property type in order to
                // -- determine which type of TomlNode to create
                if (fieldType == typeof(string) || (fieldType.GetInterface(nameof(IEnumerable)) == null && !fieldType.IsArray))
                {
                    TomlNode valueNode = GetTomlNode(fieldValue, fieldType);
                    valueNode.Comment = comment;

                    tomlData.Add(new SortNode { Name = field.Name, SortOrder = sortOrder ?? -1, Value = valueNode });
                }
                else if (fieldType.GetInterface(nameof(IEnumerable)) != null && fieldType.GetInterface(nameof(IDictionary)) != null)
                {
                    IDictionary typeValue = fieldValue as IDictionary;
                    if (typeValue == null) continue;

                    Type[] dictTypeArguments = fieldType.IsGenericType && !fieldType.IsGenericTypeDefinition
                        ? fieldType.GetGenericArguments()
                        : Type.EmptyTypes;

                    Type kType = dictTypeArguments[0];
                    Type vType = dictTypeArguments[1];

                    TomlNode dictionaryNode = CreateTomlDictionary(kType, vType, typeValue, fieldType);
                    dictionaryNode.Comment = comment;

                    tomlData.Add(new SortNode { Name = field.Name, SortOrder = sortOrder ?? -1, Value = dictionaryNode });
                }
                else
                {
                    IList propAsList = fieldValue as IList;
                    TomlArray tomlArray = new TomlArray { Comment = comment }; // @formatter:off
                    Type propArgType = fieldType.GetElementType() ?? fieldType.GetGenericArguments().FirstOrDefault();

                    if (propAsList == null) 
                    { 
                        throw new TommySerializerException($"{field.Name} could not be cast as IList.");
                    } // @formatter:on

                    for (int i = 0; i < propAsList.Count; i++)
                    {
                        if (propAsList[i] == null) throw new ArgumentNullException($"Error: collection value cannot be null");
                        tomlArray.Add(GetTomlNode(propAsList[i], propArgType));
                    }

                    tomlData.Add(new SortNode { Name = field.Name, SortOrder = sortOrder ?? -1, Value = tomlArray });
                }
            }
        }

        internal static void ProcessFieldsFromFile(TomlNode tableData, List<string> tableKeys, FieldInfo[] fields, object dataClass)
        {
            for (int k = 0; k < tableKeys.Count; k++)
            {
                string key = tableKeys[k];
                var field = fields.FirstOrDefault((prop) => {
                    var altName = (Attribute.GetCustomAttribute(prop, typeof(TommyValue), false) as TommyValue)?.Name;
                    return altName == key || prop.Name == key;
                });

                if (field == null) continue;

                Type fieldType = field.FieldType;

                TomlNode node = tableData[key];
                if (!node.HasValue && !node.IsTable) continue;

                // -- Determine if property is not a collection or table -------
                if (fieldType == typeof(string) && tableData[key].IsString || !tableData[key].IsArray && !tableData[key].IsTable)
                {
                    dataClass.SetFieldValue(key, GetValueByType(tableData[key], fieldType));
                }
                // -- Determine if property is a Toml Table/IDictionary --------
                else if (node.IsTable)
                {

                    if (fieldType.GetInterface(nameof(IEnumerable)) != null && fieldType.GetInterface(nameof(IDictionary)) != null)
                    {
                        dataClass.SetFieldValue(key, CreateGenericDictionary(tableData[key], fieldType));
                        continue;
                    }

                    if (!fieldType.IsClass)
                    {
                        throw new TommySerializerException($"Could not deserialize TomlTable into `{field.Name}` of type {fieldType.Name}");
                    }

                    var fieldDataClass = Activator.CreateInstance(fieldType) ?? throw new TommySerializerException($"Unable to instantiate {fieldType.Name}");
                    field.SetValue(dataClass, fieldDataClass);

                    List<string> fieldTableKeys = node.Keys.ToList();

                    // -- Process Properties -------------------------------------------
                    PropertyInfo[] dataClassProperties = fieldType.GetProperties(bindingFlags)
                        .Where(x => fieldTableKeys.Contains(x.Name))
                        .ToArray();

                    ProcessPropertiesFromFile(node, fieldTableKeys, dataClassProperties, fieldDataClass);

                    // -- Process Fields -----------------------------------------------
                    FieldInfo[] dataClassFields = fieldType.GetFields(bindingFlags)
                        .Where(x => !x.Name.Contains("k__BackingField") && fieldTableKeys.Contains(x.Name))
                        .ToArray();

                    ProcessFieldsFromFile(node, fieldTableKeys, dataClassFields, fieldDataClass);
                }
                else
                {
                    if (fieldType.GetInterface(nameof(IEnumerable)) == null) continue;

                    Type valueType = fieldType.GetElementType() ?? fieldType.GetGenericArguments().FirstOrDefault();
                    if (valueType == null)
                    {
                        throw new TommySerializerException($"Could not find argument type for property: {fieldType.Name}.");
                    }

                    TomlNode[] array = tableData[key].AsArray.RawArray.ToArray();

                    if (valueType.GetInterface(nameof(IConvertible)) != null)
                        dataClass.SetFieldValue(key, CreateGenericList(array, fieldType));
                    else throw new TommySerializerException($"{valueType.Name} is not able to be converted.");
                }
            }
        }

        #endregion

        #region I/O

        internal static void WriteToDisk(TomlTable tomlTable, string path)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path, false))
                { 
                    tomlTable.WriteTo(writer); 
                    writer.Flush(); 
                }
            }
            catch (Exception e) 
            {
                throw new TommySerializerException(e.Message, e);
            }
        }

        internal static MemoryStream WriteToMemory(TomlTable tomlTable)
        {
            // @formatter:off -- Writes the Toml file to disk ------------
            try
            {
                MemoryStream streamMem = new MemoryStream();
                using (StreamWriter writer = new StreamWriter(streamMem))
                { tomlTable.WriteTo(writer); writer.Flush(); }
                return streamMem;
            }
            catch (Exception e)
            {
                throw new TommySerializerException(e.Message, e);
            }
        } // @formatter:on

        internal static TomlTable ReadFromMemory(MemoryStream memoryStream)
        {
            // @formatter:off  -- Read the Toml file from Memory ------------
            try
            {
                using (Stream stream = new MemoryStream(memoryStream.ToArray(), false))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        using (TOMLParser parser = new TOMLParser(reader)) { return parser.Parse(); }
                    }
                }
            }
            catch (Exception e)
            {
                throw new TommySerializerException(e.Message, e);
            }
        } // @formatter:on

        internal static TomlTable ReadFromDisk(string path)
        {
            // @formatter:off -- Read the Toml file from Disk ------------
            try
            {
                using (Stream stream = File.OpenRead(path))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        using (TOMLParser parser = new TOMLParser(reader)) { return parser.Parse(); }
                    }
                }
            }
            catch (Exception e)
            {
                throw new TommySerializerException(e.Message, e);
            }
        } // @formatter:on

        #endregion

        #region Extension Methods

        private static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static string GetName(this Type type) =>
            type.Name.Split('.').Last().GetRange('`');

        private static string GetRange(this string s, int startIndex, char stopCharacter) // @formatter:off
        {
            string substring = "";
            for (int i = startIndex; i < s.Length; i++)
            {
                char c = s[i];
                if (c == stopCharacter) break;
                substring += c;
            }
            return substring; // @formatter:on
        }

        private static string GetRange(this string s, char stopCharacter) =>
            s.GetRange(0, stopCharacter);

        private static readonly string formatter = "0." + new string('#', 60);

        private static double FloatConverter(Type type, object obj)
        {
            return type == typeof(float)
                ? (double)Convert.ChangeType(((float)obj).ToString(formatter), TypeCode.Double)
                : (double)Convert.ChangeType(obj, TypeCode.Double);
        }

        internal static bool IsNumeric(this Type type) => (type.IsFloat() || type.IsInteger());

        internal static bool IsFloat(this Type type) => // @formatter:off
            type == typeof(float) ||
            type == typeof(double) ||
            type == typeof(decimal); // @formatter:on

        internal static bool IsInteger(this Type type) => // @formatter:off
            type == typeof(sbyte) ||
            type == typeof(byte) ||
            type == typeof(short) ||
            type == typeof(ushort) ||
            type == typeof(int) ||
            type == typeof(uint) ||
            type == typeof(long) ||
            type == typeof(ulong);
        // @formatter:on

        internal static T GetCustomAttribute<T>(this Type type) where T : Attribute
        {
            object[] attributes = type.GetCustomAttributes(typeof(T), false);
            return attributes.OfType<T>().FirstOrDefault();
        }

        #endregion

        #region Property/Field Get/Set

        internal static object GetPropertyValue(this object src, string propName) =>
            src.GetType().GetProperty(propName, bindingFlags)?.GetValue(src, null);

        internal static void SetPropertyValue<T>(this object src, string propName, T propValue) =>
            src.GetType().GetProperty(propName, bindingFlags)?.SetValue(src, propValue, null);

        internal static object GetFieldValue(this object src, string fieldName) =>
            src.GetType().GetField(fieldName, bindingFlags)?.GetValue(src);

        internal static void SetFieldValue<T>(this object src, string fieldName, T propValue) =>
            src.GetType().GetField(fieldName, bindingFlags)?.SetValue(src, propValue);

        #endregion

        #region TomlNodes

        internal static List<SortNode> SortNodes(this List<SortNode> tomlData, int maxSortInt)
        {
            for (int i = 0; i < tomlData.Count; i++)
            {
                SortNode n = tomlData[i];
                if (n.SortOrder > -1) continue;
                tomlData[i] = new SortNode { SortOrder = maxSortInt + 1, Value = n.Value, Name = n.Name };
            }

            return tomlData.OrderBy(n => n.SortOrder).ToList();
        }

        internal static object GetNodeValue(this TomlNode node, TypeCode typeCode) // @formatter:off
        {
            return node switch
            {
                { IsBoolean: true } => node.AsBoolean.Value,
                { IsString: true } => node.AsString.Value,
                { IsFloat: true } => Convert.ChangeType(node.AsFloat.Value, typeCode),
                { IsInteger: true } => Convert.ChangeType(node.AsInteger.Value, typeCode),
                { IsDateTimeOffset: true } => node.AsDateTimeOffset.Value,
                { IsDateTimeLocal: true } => node.AsDateTimeLocal.Value,
                _ => throw new ArgumentOutOfRangeException(nameof(node), node, null)
            };  // @formatter:on
        }

        internal static object GetValueByType(this TomlNode node, Type propertyType) // @formatter:off
        {
            return node switch
            {
                { IsBoolean: true } => node.AsBoolean.Value,
                { IsString: true } => node.AsString.Value,
                { IsFloat: true } => Convert.ChangeType(node.AsFloat.Value, propertyType),
                { IsInteger: true } => Convert.ChangeType(node.AsInteger.Value, propertyType),
                { IsDateTimeOffset: true } => node.AsDateTimeOffset.Value,
                { IsDateTimeLocal: true } => node.AsDateTimeLocal.Value,
                _ => throw new ArgumentOutOfRangeException(nameof(node), node, null)
            }; // @formatter:on
        }

        internal static TomlNode GetTomlNode(this object obj, Type valueType = null)
        {
            valueType ??= obj.GetType();

            return valueType switch
            {
                { } v when v == typeof(bool) => new TomlBoolean { Value = (bool)obj },
                { } v when v == typeof(string) => new TomlString { Value = (string)obj != null ? obj.ToString() : "" },
                { } v when v.IsFloat() => new TomlFloat { Value = FloatConverter(valueType, obj) },
                { } v when v.IsInteger() => new TomlInteger { Value = (long)Convert.ChangeType(obj, TypeCode.Int64) },
                { } v when v == typeof(DateTime) => new TomlDateTimeLocal { Value = (DateTime)obj },
                _ => throw new Exception($"Was not able to process item {valueType.Name}")
            }; // @formatter:on
        }

        #endregion

        #region Generic Creation

        internal static object CreateGenericList(TomlNode[] array, Type propertyType)
        {
            Type valueType = propertyType.GetElementType() ?? propertyType.GetGenericArguments().FirstOrDefault();
            if (valueType == null)
            {
                throw new TommySerializerException($"Could not find argument type for property: {propertyType.Name}.");
            }

            Type listType;
            IList list = (IList)Activator.CreateInstance(listType = typeof(List<>).MakeGenericType(valueType));

            foreach (TomlNode value in array)
            {
                if (value == null) continue;

                TypeCode typeCode = (TypeCode)Enum.Parse(typeof(TypeCode), valueType.Name);
                object nodeValue = value.GetNodeValue(typeCode);
                if (nodeValue != null) list.Add(nodeValue);
                else throw new TommySerializerException($"{propertyType.Name} value is null.");
            }

            return propertyType.IsArray ? listType.GetMethod("ToArray")?.Invoke(list, null) : list;
        }

        internal static object CreateGenericDictionary(TomlNode tableData, Type propertyType)
        {
            Type[] valueType = propertyType.GetGenericArguments();
            if (valueType.Length < 2)
            {
                throw new TommySerializerException($"Warning: Could not find argument type for property: {propertyType.Name}.");
            }

            string[] tableKeys = tableData.AsTable.Keys.ToArray();

            TomlArray dictionaryKeys = tableData[tableKeys.FirstOrDefault(x => x.EndsWith("Keys"))].AsArray;
            TomlArray dictionaryValues = tableData[tableKeys.FirstOrDefault(x => x.EndsWith("Values"))].AsArray;

            IDictionary dictionary = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(valueType));

            if (dictionaryKeys != null && dictionaryValues != null)
                for (int i = 0; i < dictionaryKeys.ChildrenCount; i++)
                {
                    TypeCode keyTypeCode = (TypeCode)Enum.Parse(typeof(TypeCode), valueType[0].Name);
                    TypeCode valueTypeCode = (TypeCode)Enum.Parse(typeof(TypeCode), valueType[1].Name);
                    dictionary.Add(GetNodeValue(dictionaryKeys[i], keyTypeCode), GetNodeValue(dictionaryValues[i], valueTypeCode));
                }
            else
            {
                throw new TommySerializerException($"Warning: Dictionary {propertyType.Name} data is missing or incorrectly formatted.");
            }

            return dictionary;
        }

        private static TomlNode CreateTomlDictionary<TKey, TValue>(TKey tKey, TValue tValue, IDictionary dictionary, Type property)
        {
            TomlTable tomlDataTable = new TomlTable();

            TomlArray dictKeys = new TomlArray();
            TomlArray dictValues = new TomlArray();

            Type kType = tKey as Type;
            Type vType = tValue as Type;

            IDictionary dictInstance = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(kType, vType));
            dictInstance = dictionary;

            int i = 0;
            foreach (DictionaryEntry kv in dictInstance)
            {
                dictKeys[i] = GetTomlNode(kv.Key);
                dictValues[i] = GetTomlNode(kv.Value);
                i++;
            }

            tomlDataTable.Add($"{property.GetName()}Keys", dictKeys);
            tomlDataTable.Add($"{property.GetName()}Values", dictValues);

            return tomlDataTable;
        }

        #endregion
    }

    #region Data Types

    internal struct SortNode
    {
        public string Name { get; set; }
        public TomlNode Value { get; set; }
        public int SortOrder { get; set; }
    }

    #endregion

    #region Attribute Classes

    /// <summary>
    /// Designates a class as a Toml Root File
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TommyRoot : Attribute
    {
        /// <summary> Designates a class as a Toml Root File </summary>
        public TommyRoot()
        { 
        }
    }

    /// <summary>
    /// Designates a class as a Toml Table and applies all contained properties as children of that table
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TommyTableName : Attribute
    {
        /// <summary>
        /// String value which will be used as the Toml Table name
        /// </summary>
        public string TableName { get; }

        /// <summary> Designates a class as a Toml Table and applies all contained properties as children of that table </summary>
        /// <param name="tableName">String value which will be used as the Toml Table name</param>
        public TommyTableName(string tableName) => TableName = tableName;
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TommyValue : Attribute
    {
        /// <summary>
        /// Alternative name for the property|field
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Int value representing the order in which this item will appear in the Toml file
        /// Determines the order in which the properties will be written to file, sorted by numeric value with 0 being
        /// the first entry but below the table name (if applicable).
        /// </summary>
        public int SortOrder { get; }

        /// <summary>  </summary>
        /// <param name="sortOrder">Int value representing the order in which this item will appear in the Toml file</param>
        public TommyValue(string name = null, int sortOrder = -1) => (Name, SortOrder) = (name, sortOrder);
    }

    /// <summary>
    ///  Adds a toml comment to a property or field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TommyComment : Attribute
    {
        /// <summary>
        /// String value which will be used as a comment for the property/field
        /// </summary>
        public string Value { get; }

        /// <summary> Adds a toml comment to a property or field </summary>
        /// <param name="comment">String value which will be used as a comment for the property/field</param>
        public TommyComment(string comment) => Value = comment;
    }

    /// <summary> When applied to a private property, the property will be included when loading or saving Toml to disk </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TommyInclude : Attribute
    {
    }

    /// <summary> When applied to a property, the property will be ignored when loading or saving Toml to disk </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TommyIgnore : Attribute
    {
    }

    #endregion
}
