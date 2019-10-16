using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InscricoesOnline.Models
{
    [Table("TBL_CHAVE_PARTICIPANTE")]
    public class ChaveParticipante
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long ChaveId { get; set; }
        public virtual Chave Chave { get; set; }

        public long InscricaoModalidadeId { get; set; }
        public virtual InscricaoModalidade InscricaoModalidade { get; set; }

        public int Ordem { get; set; }

        public int Colocacao { get; set; }

        public long EventoId { get; set; }
        public virtual Evento Evento { get; set; }

        public bool Ativo { get; set; }
    }
}