namespace ProjetASPMVC1.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using ProjetASPMVC1.Models;
    internal sealed class Configuration : DbMigrationsConfiguration<ProjetASPMVC1.Models.Projet_ContextBD>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ProjetASPMVC1.Models.Projet_ContextBD context)
        {

            var fil = new List<Filiere>();
            fil.Add(new Filiere { nom_fil = "G�nie Informatique" });
            fil.Add(new Filiere { nom_fil = "G�nie Proc�d�s et Mat�riaux C�ramiques" });
            fil.Add(new Filiere { nom_fil = "G�nie T�l�communications et R�seaux" });
            fil.Add(new Filiere { nom_fil = "G�nie Industriel" });
            context.Filieres.AddRange(fil);
            base.Seed(context);
        }
    }
}
