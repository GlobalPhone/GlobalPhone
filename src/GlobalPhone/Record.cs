using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
namespace GlobalPhone
{
    public class Record
    {
        private readonly object[] _data;

		protected object[] AsArray(object value)
		{
			IEnumerable enumerable ;
			if ((enumerable = value as IEnumerable) != null) {
				return enumerable.Cast<Object>().ToArray();
			} else {
				throw new Exception ("Unknown type: "+value.GetType().Name);
			}
		}

        public Record(object data)
        {
			_data = AsArray (data);
        }
		protected object[] FieldAsArray(int index)
		{
			return AsArray(_data[index]);
		}
        protected T Field<T>(int index, T fallback=default(T))
        {
            try
            {
                if (_data.Length <= index)
                    return fallback;
                var data = (T)_data[index];
                return data;
            }
            catch (Exception e)
            {
                throw new Exception("Index: "+index,e);
            }
        }

        protected TRet Field<T, TRet>(int index, Func<T, TRet> block)
        {
            try
            {
                if (_data.Length <= index)
                    return default(TRet);
                var data = (T)_data[index];
                return block(data);
            }
            catch (Exception e)
            {
                throw new Exception("Index: " + index, e);
            }
        }
    }
}