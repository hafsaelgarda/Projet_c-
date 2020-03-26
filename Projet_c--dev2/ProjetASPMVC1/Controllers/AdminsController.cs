﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ProjetASPMVC1.Models;

namespace ProjetASPMVC1.Controllers
{
    public class AdminsController : Controller
    {
        private Projet_ContextBD db = new Projet_ContextBD();

        public ActionResult LoginAdmin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authorise(string login, string password)
        {
            //if (ModelState.IsValid)
            //    {
            using (Projet_ContextBD db = new Projet_ContextBD())
            {

                var userDetail = db.Admins.Where(x => x.Login == login && x.password == password).FirstOrDefault();
                if (userDetail == null)
                {

                    // user.LoginErrorMsg = "Invalid UserName or Password";
                    Response.Write("<script>alert(\'données incorrect\');</" + "script>");
                    return View("LoginAdmin");
                }
                else
                {

                    Session["login"] = userDetail.Login;
                    Session["password"] = userDetail.password;
                    int nb3eme = 0;
                    int nb4eme = 0;
                    int nbIns = 0;
                    foreach (var cand in db.Candidats.ToArray())
                    {
                        if (cand.niveau.Equals("4eme"))
                        {
                            nb4eme++;
                        }
                        else if (cand.niveau.Equals("3eme"))
                        {
                            nb3eme++;
                        }

                        nbIns++;
                    }

                    ViewBag.niv3 = Convert.ToString(nb3eme);
                    ViewBag.niv4 = Convert.ToString(nb4eme);
                    ViewBag.ut = Convert.ToString(nbIns.ToString());

                    return View("Index");

                }

            }






        }

        // chart 4eme
        public ActionResult MyChartNiveau4()
        {

            var x = from p in db.Filieres select p.nom_fil;
            var x2 = from p in db.Filieres select p;
            var y = from p in db.Candidats select p;

            List<int> cmpt = new List<int>();
            List<string> fil = new List<string>();




            foreach (var t2 in x2)
            {
                int c = 0;
                foreach (var t in y)
                {

                    if (t2.id_fil == t.id_fil && t.niveau.Equals("4eme"))
                    {

                        c++;
                    }

                }


                cmpt.Add(c);
            }

            foreach (var t2 in x)
            {

                fil.Add(t2.ToString());

            }

            new System.Web.Helpers.Chart(width: 800, height: 200).AddTitle("Nombre des Candidats de 4eme année par filiere").AddSeries(chartType: "Column", xValue: fil, yValues: cmpt).Write("png");

            return null;
        }
        //chart de 3eme année

        public ActionResult MyChartNiveau3()
        {

            var x = from p in db.Filieres select p.nom_fil;
            var x2 = from p in db.Filieres select p;
            var y = from p in db.Candidats select p;

            List<int> cmpt = new List<int>();
            List<string> fil = new List<string>();




            foreach (var t2 in x2)
            {
                int c = 0;
                foreach (var t in y)
                {

                    if (t2.id_fil == t.id_fil && t.niveau.Equals("3eme"))
                    {

                        c++;
                    }

                }


                cmpt.Add(c);
            }

            foreach (var t2 in x)
            {

                fil.Add(t2.ToString());

            }

            new System.Web.Helpers.Chart(width: 800, height: 200).AddTitle("Nombre des Candidats de 3eme année par filiere").AddSeries(chartType: "Column", xValue: fil, yValues: cmpt).Write("png");
            return null;
        }

        // GET: Admins
        public ActionResult Index(string id)
        {
            int nb3eme = 0;
            int nb4eme = 10;
            int nbIns = 0;
            foreach (var cand in db.Candidats.ToArray())
            {
                if (cand.niveau.Equals("4eme"))
                {
                    nb4eme++;
                }
                else if (cand.niveau.Equals("3eme"))
                {
                    nb3eme++;
                }

                nbIns++;
            }

            ViewBag.niv3 = Convert.ToString(nb3eme);
            ViewBag.niv4 = Convert.ToString(nb4eme);
            ViewBag.ut = Convert.ToString(nbIns.ToString());
            return View();
        }

        public ActionResult Convoquer()
        {

            return View();
        }

        [HttpPost]
        public PartialViewResult ConvoqueDossier(string CIN)
        {

            Candidat et = db.Candidats.Find(CIN);
            
            if (et.n_dossier.Equals(""))
            {
                Random rnd = new Random();
              
                rnd.Next(1,100);
                et.n_dossier ="M"+ Convert.ToString(et.CNE) + "end";
                db.SaveChanges();
                return PartialView("_ConvoqueDossier", et);

            }
            else
            {
                Response.Write("<script>alert(\'Erreur le Candidat déja convoquer CIN non valide !!!!!\');</" + "script>");
                return PartialView("_ConvoqueDossierErr", et);
            }

           
        }
        public ActionResult LogOut()
        {
            Session.Clear();
           
            return RedirectToAction("Index","Home");
        }


        public ActionResult correction(String niveau,String idfil,String msg)
        {
            int id = Convert.ToInt32(idfil);
            var candidats = db.Candidats.Include(c => c.Diplome).Include(c => c.Filiere).Include(c => c.Notes);
            candidats = candidats.Where(c => c.niveau == niveau && c.id_fil == id);
            ViewBag.msg = msg;
            return View(candidats.ToList());
           
        }

        public ActionResult correction2()
        {

            var Notes1 = Request["item.Notes.notemath"].ToString().Split(',');
            var Notes2 = Request["item.Notes.notespec"].ToString().Split(',');
            var CINS = Request["item.CIN"].ToString().Split(',');
            String d;

            for (int i = 0; i < CINS.Length; i++)
            {
                d = CINS[i].ToString();
                Candidat cand = db.Candidats.Find(d);
                int idnote = cand.id_note;
                Notes n = db.Notes.Find(idnote);
                n.notemath= Double.Parse(Notes1[i]);
                n.notespec = Double.Parse(Notes2[i]);
                n.note_concours = (n.notemath + n.notespec) / 2;
                db.SaveChanges();

            }
            String cin = CINS[0];
            Candidat c = db.Candidats.Find(cin);
            String niveau = c.niveau;
            String idfil = c.id_fil.ToString();
            String msg = "affectation des notes du concours avec succés";


            return RedirectToAction("correction", new { niveau = niveau, idfil=idfil,msg=msg });
        }
    }
}
