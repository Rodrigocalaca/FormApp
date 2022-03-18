using ProjetoDeEstagio2;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EM.Repository
{
    public abstract class RepositorioAbstrato<T> where T : IEntidade
    {
        public abstract void Add(T aluno);
        public virtual void Remove(T aluno) { }
        public virtual void Update(T aluno) { }
        public virtual IEnumerable<T> GetAll(T aluno) { return null; }
        public virtual IEnumerable<T> Get(Expression<Func<T, bool>> predicate) { return null; } // estudar
    }
}