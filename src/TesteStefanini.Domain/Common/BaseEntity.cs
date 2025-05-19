using System;

namespace TesteStefanini.Domain.Common
{
    // Classe base para entidades com campos de auditoria
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            DataCadastro = DateTime.Now;
        }
    }
}
