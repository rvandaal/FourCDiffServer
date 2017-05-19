using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FourCDiffServer.Tests {
    public class TestDbSet<T> : DbSet<T>, IQueryable, IEnumerable<T>
        where T : class {

        private readonly IEnumerable<PropertyInfo> keys;
        private readonly ObservableCollection<T> data;
        private readonly IQueryable query;

        public TestDbSet() {
            keys = typeof(T).GetProperties()
                                  .Where(p => Attribute.IsDefined(p, typeof(KeyAttribute))
                                           || "Id".Equals(p.Name, StringComparison.Ordinal))
                                  .ToList();
            data = new ObservableCollection<T>();
            query = data.AsQueryable();
        }

        public override T Add(T item) {
            data.Add(item);
            return item;
        }

        public override T Remove(T item) {
            data.Remove(item);
            return item;
        }

        public override T Attach(T item) {
            data.Add(item);
            return item;
        }

        public override T Create() {
            return Activator.CreateInstance<T>();
        }

        public override TDerivedEntity Create<TDerivedEntity>() {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public override T Find(params object[] keyValues) {
            if (keyValues == null)
                throw new ArgumentNullException("keyValues");
            if (keyValues.Any(k => k == null))
                throw new ArgumentOutOfRangeException("keyValues");
            if (keyValues.Length != keys.Count())
                throw new ArgumentOutOfRangeException("keyValues");

            return data.SingleOrDefault(i =>
                keys.Zip(keyValues, (k, v) => v.Equals(k.GetValue(i)))
                    .All(r => r)
            );
        }

        public override Task<T> FindAsync(params object[] keyValues) {
            return Task.FromResult(Find(keyValues));
        }

        public override Task<T> FindAsync(CancellationToken cancellationToken, params object[] keyValues) {
            return Task.FromResult(Find(keyValues));
        }

        public override ObservableCollection<T> Local {
            get { return new ObservableCollection<T>(data); }
        }

        Type IQueryable.ElementType {
            get { return query.ElementType; }
        }

        System.Linq.Expressions.Expression IQueryable.Expression {
            get { return query.Expression; }
        }

        IQueryProvider IQueryable.Provider {
            get { return query.Provider; }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return data.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return data.GetEnumerator();
        }
    }
}