using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InscricoesOnline.Models
{
    [Table("TBL_EVENTOS")]
    public class Evento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(60)]
        public string Titulo { get; set; }

        [AllowHtml]
        public string Descricao { get; set; }

        [Required]
        public bool AtivaInscricao  { get; set; }

        [MaxLength(30)]
        public string Login { get; set; }

        [MaxLength(30)]
        public string Senha { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataInicio { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataFim { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataInicioInscricoes { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataFimInscricoes { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataFimAlteracoes { get; set; }

        [Required]
        public bool Ativo { get; set; }

        [MaxLength(10)]
        public String Imagem { get; set; }

        [MaxLength(100)]
        public string Cidade { get; set; }

        [MaxLength(10)]
        public string Estado { get; set; }

        [MaxLength(120)]
        public string Local { get; set; }
    }
}