using System;

namespace GlobalPhone
{
    public class Record
    {
        private readonly object[] _data;

        public Record(object data)
        {
            _data = (object[])data;
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