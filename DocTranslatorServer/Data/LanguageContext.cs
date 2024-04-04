using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DocTranslatorServer.Models;

    public class LanguageContext : DbContext
    {
        public LanguageContext (DbContextOptions<LanguageContext> options)
            : base(options)
        {
        }

        public DbSet<DocTranslatorServer.Models.Language> Language { get; set; } = default!;
    }
