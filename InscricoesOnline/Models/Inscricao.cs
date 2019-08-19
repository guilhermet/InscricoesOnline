using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InscricoesOnline.Models
{
    [Table("TBL_INSCRICOES")]
    public class Inscricao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long EventoId { get; set; }
        public virtual Evento Evento { get; set; }

        public long AcademiaId { get; set; }
        public virtual Equipe Academia { get; set; }

        public long FiliadoId { get; set; }
        public virtual Atleta Filiado { get; set; }

        public DateTime DataInscricao { get; set; }

        public virtual List<InscricaoModalidade> InscricaoModalidade { get; set; }
    }
}