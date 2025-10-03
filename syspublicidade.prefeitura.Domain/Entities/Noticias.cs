using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syspublicidade.prefeitura.Domain.Entities
{
    public enum CategoriaNoticia
    {
        Esportes,
        Cultura,
        Policial
    }

    public class Noticias
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Conteudo { get; set; } = string.Empty;
        public CategoriaNoticia Categoria { get; set; }
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        public Guid UsuarioId { get; set; }
        public Usuarios Usuario { get; set; } = default!;
    }
}
