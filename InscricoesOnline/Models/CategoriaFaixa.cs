using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InscricoesOnline.Models
{
    [Table("TBL_CATEGORIAS_FAIXA")]
    public class CategoriaFaixa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        public String Titulo { get; set; }

        [Required]
        public long ModalidadeId { get; set; }
        public virtual Modalidade Modalidade { get; set; }

        public long FaixaIdFaixaInicial { get; set; }
        [ForeignKey("FaixaIdFaixaInicial")]
        public virtual Faixa FaixaInicial { get; set; }

        public long FaixaIdFaixaFinal { get; set; }
        [ForeignKey("FaixaIdFaixaFinal")]
        public virtual Faixa FaixaFinal { get; set; }

        public bool Ativo { get; set; }

        public long EventoId { get; set; }
        public virtual Evento Evento { get; set; }
    }
}