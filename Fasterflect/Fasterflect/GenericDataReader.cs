using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Fasterflect
{
	/// <summary>
	/// Generic <see cref="DbDataReader"/> for use with ADO.NET SQL select commands. Converts from a <see cref="DataTable"/> to <see cref="IEnumerable{T}"/>.
	/// </summary>
	/// <typeparam name="T">The class type to read data into.</typeparam>
	/// <see href="https://github.com/mgravell/fast-member/blob/master/FastMember/ObjectReader.cs"/>
	public class GenericDataReader<T> : DbDataReader where T : class
	{
		private IEnumerator enumerator;
		private readonly PropertyInfo[] pinfos;
		private readonly Type[] types;
		private readonly BitArray allowNull;
		private object current = null;
		private bool active = true;
		private const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.DeclaredOnly;

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericDataReader{T}"/> class.
		/// </summary>
		/// <param name="source">The <see cref="IEnumerable{T}"/> to read.</param>
		/// <param name="flags">The <see cref="BindingFlags"/> for the type's properties.</param>
		/// <param name="props">The property names that will be read. If this is empty then all properties will be read.</param>
		public GenericDataReader(IEnumerable<T> source, BindingFlags flags = BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.DeclaredOnly, params string[] props)
			: this(source, typeof(T).GetProperties(flags).Where(pinfo => props.Contains(pinfo.Name)).ToArray())
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericDataReader{T}"/> class.
		/// </summary>
		/// <param name="source">The <see cref="IEnumerable{T}"/> to read.</param>
		/// <param name="columns">The names of the table columns that match the properties.</param>
		/// <param name="props">The properties that will be read. If this is empty then all properties will be read.</param>
		public GenericDataReader(IEnumerable<T> source, PropertyInfo[] props)
		{
			pinfos = props.Length == 0 ? typeof(T).GetProperties(DefaultBindingFlags) : props;
			enumerator = source.GetEnumerator();
			types = new Type[pinfos.Length];
			allowNull = new BitArray(pinfos.Length);
			for (int i = 0; i < pinfos.Length; i++) {
				Type propertyType = pinfos[i].PropertyType;
				Type ty = Nullable.GetUnderlyingType(propertyType);
				allowNull[i] = ty != null || propertyType.IsClass;
				types[i] = ty ?? propertyType;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericDataReader{T}"/> class.
		/// </summary>
		/// <param name="source">The <see cref="IEnumerable{T}"/> to read.</param>
		/// <param name="props">The property names that will be read. If this is empty then all properties will be read.</param>
		public GenericDataReader(IEnumerable<T> source, params string[] props)
			: this(source, DefaultBindingFlags, props.Length == 0 ? typeof(T).GetProperties(DefaultBindingFlags).Select(p => p.Name).ToArray() : props) { }

		/// <summary>
		/// Not implemented.
		/// </summary>
		public override int Depth => throw new NotImplementedException();

		/// <summary>
		/// Returns a <see cref="DataTable"/> that describes the column metadata of the <see cref="DbDataReader"/>.
		/// </summary>
		/// <returns>A <see cref="DataTable"/> that describes the column metadata.</returns>
		public override DataTable GetSchemaTable()
		{
			DataTable table = new DataTable
			{
				Columns =
				{
					{ "ColumnOrdinal", typeof(int) },
					{ "ColumnName", typeof(string) },
					{ "DataType", typeof(Type) },
					{ "ColumnSize", typeof(int) },
					{ "AllowDBNull", typeof(bool) },
                    ////{ "NumericPrecision", typeof(short) },
                    ////{ "NumericScale", typeof(short) },
                    ////{ "IsUnique", typeof(bool) },
                    ////{ "IsKey", typeof(bool) },
                    ////{ "IsIdentity", typeof(bool) },
                    ////{ "IsHidden", typeof(bool) },
                    ////{ "ProviderType", typeof(SqlDbType) },
                    ////{ "ProviderSpecificDataType", typeof(Type) },
                    ////{ "IsExpression", typeof(bool) },
                    ////{ "IsAliased", typeof(bool) },
                    ////{ "IsAutoIncrement", typeof(bool) },
                    ////{ "IsRowVersion", typeof(bool) },
                    ////{ "IsLongBlob", typeof(bool) },
                    ////{ "IsReadOnly", typeof(bool) },
                    ////{ "NonVersionedProviderType", typeof(SqlDbType) },
                    ////{ "DataTypeName", typeof(string) },
                    ////{ "IsColumnSet", typeof(bool) }
                }
			};

			object[] rowData = new object[5];
			for (int i = 0; i < pinfos.Length; i++) {
				rowData[0] = i;
				rowData[1] = pinfos[i].Name;
				rowData[2] = types[i];
				rowData[3] = -1;
				rowData[4] = allowNull[i];
				table.Rows.Add(rowData);
			}
			return table;
		}

		/// <summary>
		/// Closes the <see cref="DbDataReader"/> object.
		/// </summary>
		public override void Close()
		{
			Shutdown();
		}

		/// <summary>
		/// Gets a value that indicates whether this <see cref="DbDataReader"/> contains one or more rows.
		/// </summary>
		/// <returns>True if the <see cref="DbDataReader"/> contains one or more rows; otherwise false.</returns>
		public override bool HasRows => active;

		/// <summary>
		/// Advances the reader to the next result when reading the results of a batch of statements.
		/// </summary>
		/// <returns>True if there are more result sets; otherwise false.</returns>
		public override bool NextResult()
		{
			active = false;
			return false;
		}

		/// <summary>
		/// Advances the reader to the next record in a result set.
		/// </summary>
		/// <returns>True if there are more rows; otherwise false.</returns>
		public override bool Read()
		{
			if (active) {
				if (enumerator != null && enumerator.MoveNext()) {
					current = enumerator.Current;
					return true;
				}
				active = false;
				current = null;
			}
			return false;
		}

		/// <summary>
		/// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
		/// </summary>
		/// <returns>The number of rows changed, inserted, or deleted. -1 for SELECT statements;
		/// 0 if no rows were affected or the statement failed.</returns>
		public override int RecordsAffected => 0;

		/// <summary>
		/// Releases the managed resources used by the <see cref="DbDataReader"/> and optionally releases the unmanaged resources.
		/// </summary>
		/// <param name="disposing">True to release managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
				Shutdown();
		}

		private void Shutdown()
		{
			active = false;
			current = null;
			if (enumerator is IDisposable tmp)
				tmp.Dispose();
			enumerator = null;
		}

		/// <summary>
		/// Gets the number of columns in the current row.
		/// </summary>
		/// <returns>The number of columns in the current row.</returns>
		public override int FieldCount => pinfos.Length;

		/// <summary>
		/// Gets a value indicating whether the <see cref="DbDataReader"/> is closed.
		/// </summary>
		/// <returns>True if the <see cref="DbDataReader"/> is closed; otherwise false.</returns>
		public override bool IsClosed => enumerator == null;

		/// <summary>
		/// Gets the value of the specified column as a Boolean.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		public override bool GetBoolean(int ordinal)
		{
			return (bool) this[ordinal];
		}

		/// <summary>
		/// Gets the value of the specified column as a byte.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		public override byte GetByte(int ordinal)
		{
			return (byte) this[ordinal];
		}

		/// <summary>
		/// Reads a stream of bytes from the specified column, starting at location indicated by dataOffset,
		/// into the buffer, starting at the location indicated by bufferOffset.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
		/// <param name="buffer">The buffer into which to copy the data.</param>
		/// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
		/// <param name="length">The maximum number of characters to read.</param>
		/// <returns>The actual number of bytes read.</returns>
		public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
		{
			byte[] s = (byte[]) this[ordinal];
			int available = s.Length - (int) dataOffset;
			if (available <= 0)
				return 0;

			int count = Math.Min(length, available);
			Buffer.BlockCopy(s, (int) dataOffset, buffer, bufferOffset, count);
			return count;
		}

		/// <summary>
		/// Gets the value of the specified column as a single character.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		public override char GetChar(int ordinal)
		{
			return (char) this[ordinal];
		}

		/// <summary>
		/// Reads a stream of characters from the specified column, starting at location indicated by dataOffset,
		/// into the buffer, starting at the location indicated by bufferOffset.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
		/// <param name="buffer">The buffer into which to copy the data.</param>
		/// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
		/// <param name="length">The maximum number of characters to read.</param>
		/// <returns>The actual number of characters read.</returns>
		public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
		{
			string s = (string) this[ordinal];
			int available = s.Length - (int) dataOffset;
			if (available <= 0)
				return 0;

			int count = Math.Min(length, available);
			s.CopyTo((int) dataOffset, buffer, bufferOffset, count);
			return count;
		}

		/// <summary>
		/// Not Supported.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>A <see cref="DbDataReader"/> object.</returns>
		protected override DbDataReader GetDbDataReader(int ordinal)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets name of the data type of the specified column.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>A string representing the name of the data type.</returns>
		public override string GetDataTypeName(int ordinal)
		{
			return types[ordinal].Name;
		}

		/// <summary>
		/// Gets the value of the specified column as a <see cref="DateTime"/>.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		public override DateTime GetDateTime(int ordinal)
		{
			return (DateTime) this[ordinal];
		}

		/// <summary>
		/// Gets the value of the specified column as a <see cref="decimal"/>.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		public override decimal GetDecimal(int ordinal)
		{
			return (decimal) this[ordinal];
		}

		/// <summary>
		/// Gets the value of the specified column as a <see cref="double"/>.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		public override double GetDouble(int ordinal)
		{
			return (double) this[ordinal];
		}

		/// <summary>
		/// Gets the data type of the specified column.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		public override Type GetFieldType(int ordinal)
		{
			return types[ordinal];
		}

		/// <summary>
		/// Gets the value of the specified column as a <see cref="float"/>.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		public override float GetFloat(int ordinal)
		{
			return (float) this[ordinal];
		}

		/// <summary>
		/// Gets the value of the specified column as a <see cref="Guid"/>.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		public override Guid GetGuid(int ordinal)
		{
			return (Guid) this[ordinal];
		}

		/// <summary>
		/// Gets the value of the specified column as a <see cref="short"/>.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		public override short GetInt16(int ordinal)
		{
			return (short) this[ordinal];
		}

		/// <summary>
		/// Gets the value of the specified column as an <see cref="int"/>.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		public override int GetInt32(int ordinal)
		{
			return (int) this[ordinal];
		}

		/// <summary>
		/// Gets the value of the specified column as a <see cref="long"/>.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		public override long GetInt64(int ordinal)
		{
			return (long) this[ordinal];
		}

		/// <summary>
		/// Gets the name of the column, given the zero-based column ordinal.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The name of the specified column.</returns>
		public override string GetName(int ordinal)
		{
			return pinfos[ordinal].Name;
		}

		/// <summary>
		/// Gets the column ordinal given the name of the column.
		/// </summary>
		/// <param name="name">The name of the column.</param>
		/// <returns>The zero-based column ordinal.</returns>
		public override int GetOrdinal(string name)
		{
			for (int i = 0; i < pinfos.Length; i++) {
				if (pinfos[i].Name == name) {
					return i;
				}
			}
			throw new ArgumentException(name);
		}

		/// <summary>
		/// Gets the value of the specified column as an instance of <see cref="string"/>.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		public override string GetString(int ordinal)
		{
			return (string) this[ordinal];
		}

		/// <summary>
		/// Gets the value of the specified column as an instance of <see cref="object"/>.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		public override object GetValue(int ordinal)
		{
			return this[ordinal];
		}

		/// <summary>
		/// Returns an <see cref="IEnumerator"/> that can be used to iterate through the rows in the <see cref="DbDataReader"/>.
		/// </summary>
		/// <returns>An <see cref="IEnumerator"/> that can be used to iterate through the rows in the <see cref="DbDataReader"/>.</returns>
		public override IEnumerator GetEnumerator()
		{
			return new DbEnumerator(this);
		}

		/// <summary>
		/// Populates an <see cref="object"/>[] with the column values of the current row.
		/// </summary>
		/// <param name="values">An <see cref="object"/>[] to copy attributes to.</param>
		/// <returns>The number of instances of <see cref="object"/> in the array.</returns>
		public override int GetValues(object[] values)
		{
			for (int i = 0; i < pinfos.Length; i++)
				values[i] = pinfos[i].GetValue(current, null) ?? DBNull.Value;
			return pinfos.Length;
		}

		/// <summary>
		/// Gets a value that indicates whether the column contains nonexistent or missing values.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>True if the specified column is equivalent to <see cref="DBNull"/>; otherwise false.</returns>
		public override bool IsDBNull(int ordinal)
		{
			return this[ordinal] == DBNull.Value;
		}

		/// <summary>
		/// Gets the value of the specified column as an instance of <see cref="object"/>.
		/// </summary>
		/// <param name="name">The name of the column.</param>
		/// <returns>The value of the specified column.</returns>
		public override object this[string name] => pinfos.First(prop => prop.Name == name).GetValue(current, null) ?? DBNull.Value;

		/// <summary>
		/// Gets the value of the specified column as an instance of <see cref="object"/>.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		public override object this[int ordinal] => pinfos[ordinal].GetValue(current, null) ?? DBNull.Value;
	}
}