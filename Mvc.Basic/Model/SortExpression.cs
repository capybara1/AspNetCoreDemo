using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace AspNetCoreDemo.Mvc.Basic.Model
{
    public class SortExpression : IReadOnlyList<FieldExpression>
    {
        private readonly IReadOnlyList<FieldExpression> _fields;

        public SortExpression(IEnumerable<FieldExpression> fields)
        {
            if (fields == null) throw new System.ArgumentNullException(nameof(fields));

            _fields = fields.ToArray();
        }

        public FieldExpression this[int index] => _fields[index];

        public int Count => _fields.Count;

        public IEnumerator<FieldExpression> GetEnumerator()
        {
            return _fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _fields.GetEnumerator();
        }
    }
}