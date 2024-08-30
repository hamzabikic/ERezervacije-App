import {Component, OnDestroy, OnInit} from '@angular/core';
import {
  BrisanjeNalogaResponse,
  GostResponse, GostRezervacijaInfo,
  Grad,
  KorisnikEditRequest, LozinkaRequest, LozinkaResponse,
  Popust, RecenzijaPost,
  RezervacijaGostResponse
} from "../KorisnikKlase/KorisnikResponse";
import {HttpClient} from "@angular/common/http";
import {MojConfig} from "../Servisi/MojConfig";
import {MyAuth} from "../Servisi/MyAuth";
import {Rezultat} from "../AutentifikacijaKlase/LoginRequest";
import {isNullOrEmpty} from "../Helpers/StringHelper";
import {KorisnikEditResponse} from "../KorisnikKlase/ManagerKlase";
import {Router} from "@angular/router";

@Component({
  selector: 'app-moj-nalog-gost',
  templateUrl: './moj-nalog-gost.component.html',
  styleUrls: ['./moj-nalog-gost.component.css']
})
export class MojNalogGostComponent implements OnInit, OnDestroy {

  korisnik:GostResponse|undefined;
  dialog_edit = false;
  gradovi:Grad[]|undefined = [];
  ime="";
  prezime="";
  datumRodjenja:string ="";
  gradId =0;
  email="";
  username="";
  brojTelefona ="";
  popusti:Popust[]|undefined;
  rezervacije : RezervacijaGostResponse []|undefined;
  brojZvjezdica =0;
  listaObicnih :number[] = [];
  listaDrugih: number[] =[];
  komentar ="";
  dialog_recenzija = false;
  rezervacijaId =0;
  upozorenje="";
  rezervacija:GostRezervacijaInfo|undefined;
  dialog_detalji=false;
  staraLozinka="";
  novaLozinka="";
  dialog_lozinka =false;
  lozinka_upozorenje="";
  dialog_verifikacija_telefon = false;
  onemogucen_telefon = false;
  verifikacijski_kod="";
  preostalo=60;
  interval:any;
  slikaURL ="";
  promijenjenaSlika = false;
  otkazi_dialog= false;
  brisanje_naloga_dialog = false;

  constructor(private http:HttpClient,protected auth:MyAuth, private router:Router) { }
   async preuzmiDetalje (id:number){
    this.rezervacija = await this.http.get<GostRezervacijaInfo>(MojConfig.adresa_servera+"GetGostRezervacijaInfo?id="
      +id,
      {headers: {"my-token":this.auth.getToken()?.token!}}).toPromise();
     this.dialog_detalji=!this.dialog_detalji;
   }
   ucitajSliku() {
      let reader = new FileReader();
      // @ts-ignore
     let file = document.getElementById("slika").files[0];
     if(!file) {
       this.slikaURL="";
       this.promijenjenaSlika=false;
       return;
     }
     let fileSizeMB = file.size / (1024 * 1024);
     if(fileSizeMB>2) {
       this.slikaURL ="";
       this.promijenjenaSlika =false;
       alert("Velicina slike je prevelika! Maksimalna velicina: 2mb");
       return;
     }
      reader.onload = ()=> {
          this.slikaURL = reader.result!.toString();
          this.promijenjenaSlika=true;
        }

      reader.readAsDataURL(file);
   }
   async verifikacijaTelefona () {
    if(isNullOrEmpty(this.verifikacijski_kod)) {
      alert("Niste unijeli kod!");
      return;
    }
     let res = await this.http.get(MojConfig.adresa_servera+"VerifikujTelefon?key="
       +this.verifikacijski_kod,
       {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();
    if(res) {
      this.dialog_verifikacija_telefon= false;
      await this.ucitajKorisnika();
      clearInterval(this.interval);
      this.onemogucen_telefon=false;
    }
    else {
      alert("Kod je neispravan!");
    }
   }
  async  otvoriVerifikaciju() {
    if(!this.onemogucen_telefon) {
      let res= await this.http.get(MojConfig.adresa_servera+"SlanjeVerifikacijeTelefon",
        {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise() as boolean;
      if(!res) {
        alert("Trenutno nije moguća verifikacija, pokušajte ponovo kasnije!");
        return;
      }
      if(res) {
     this.postaviVrijeme();
     this.verifikacijski_kod ="";
     this.dialog_verifikacija_telefon = true;
     return;
      }
      alert("Neuspjesno slanje verifikacije!");
    }
   else {
     alert("Trenutno onemoguceno slanje verifikacijskog koda. Molimo pokusajte ponovo za "+this.preostalo +" sekundi!");
   }

}
   async postaviVrijeme() {
     this.preostalo=60;
       this.onemogucen_telefon=true;
    this.interval = setInterval(
      ()=> {
        this.preostalo = this.preostalo-1;
        if(this.preostalo==0) {
          clearInterval(this.interval); this.onemogucen_telefon=false;this.preostalo=0;
        }
      },1000
    );
   }
  async izbrisiRezervaciju () {
    let res = await this.http.get
    (MojConfig.adresa_servera+"IzbrisiRezervacijuGost?id="+this.rezervacijaId,
      {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();
    if(res) {
      await this.getRezervacije();
      await this.ucitajKorisnika();
      this.otkazi_dialog=!this.otkazi_dialog;
      return;
    }
    else {
      alert("Nije moguce otkazati rezervaciju!");
    }
  }
  pozadina (otkazana:boolean, ponistena:boolean,preuzeta:boolean ) {
    if (otkazana || ponistena) {
      return {
        "background-color": "rgba(255, 0, 0, 0.2)"
      };
    } else if (preuzeta) {
      return {
        "background-color": "rgb(221,255,221)"
      };
    } else {
      return {
        "background-color": "white"
      };
    }
  }

  async postojiEmail () {
    if(this.email =='')return false;
    let res = await this.http.get<Rezultat>(MojConfig.adresa_servera+"PostojiEmail?email="+
    this.email, {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();
    if(res!.postoji) {
       this.upozorenje = "Uneseni email već postoji u sistemu!";
       // @ts-ignore
      document.getElementById("email").style.border="1px solid red";
       return true;
    }
    else {
      this.upozorenje="";
      // @ts-ignore
      document.getElementById("email").style.border="1px solid rgba(169, 169, 169, 0.48)";
      return false;
    }
  }
  async postojiBrojTelefona() {
    if(this.brojTelefona=='') return false;
    let res = await this.http.get<Rezultat>(MojConfig.adresa_servera+"PostojiBrojTelefona?telefon="+
      encodeURIComponent(this.brojTelefona), {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();
    if(res!.postoji) {
      this.upozorenje = "Uneseni broj telefona već postoji u sistemu!";
      // @ts-ignore
      document.getElementById("brojTelefona").style.border="1px solid red";
      return true;
    }
    else {
      this.upozorenje="";
      // @ts-ignore
      document.getElementById("brojTelefona").style.border="1px solid rgba(169, 169, 169, 0.48)";
      return false;
    }
  }
  async postojiUsername() {
    if(this.username=='')return false;
    let res = await this.http.get<Rezultat>(MojConfig.adresa_servera+"PostojiUsername?username="+
      this.username, {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();
    if(res!.postoji) {
      this.upozorenje = "Uneseni username već postoji u sistemu!";
      // @ts-ignore
      document.getElementById("username").style.border="1px solid red";
      return true;
    }
    else {
      this.upozorenje="";
      // @ts-ignore
      document.getElementById("username").style.border="1px solid rgba(169, 169, 169, 0.48)";
      return false;
    }
  }
  isNullOrEmpty(tekst:string) {
    if(tekst =='') return true;
    let lista = tekst.split(" ");
    let text = lista.join('');
    return text.length==0;
  }
  popunjenoPolje (event:Event) {
       let element = event.target as HTMLInputElement;
       if(this.isNullOrEmpty(element.value)) {
         element.style.border="1px solid red";
       }
       else {
         element.style.border="1px solid rgba(169, 169, 169, 0.48)";
       }
  }
  provjeriPolja () {
   if(this.isNullOrEmpty(this.ime) || this.isNullOrEmpty(this.prezime) || this.isNullOrEmpty(this.brojTelefona)
     || this.isNullOrEmpty(this.username) || this.isNullOrEmpty(this.email) || this.gradId==0)
    {
      this.upozorenje="Sva polja su obavezna!";
      return true;
    }
   this.upozorenje="";
   return false;

  }
  async getRezervacije() {
    this.rezervacije = await this.http.get<RezervacijaGostResponse[]>(MojConfig.adresa_servera+"GetRezervacijeGost",
      {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();

  }
  async verifikujEmail() {
      let res = await this.http.get(MojConfig.adresa_servera+"SlanjeVerifikacijeEmail",
        {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();
      if(res) {
        alert("Provjerite svoje e-mail sanduce!");
      }
      else {
        alert("Trenutno nije moguća verifikacija, pokušajte ponovo kasnije!");
      }
  }
  async ucitajKorisnika() {
    this.korisnik = await this.http.get<GostResponse>(MojConfig.adresa_servera+"GetGost",
      {headers:{"my-token":this.auth.getToken()?.token!} }).toPromise();
    this.slikaURL= MojConfig.adresa_servera+"GetProfilnaById?id=" + this.auth.getToken()?.gost?.korisnikID;
  }
  async getPopusti() {
    this.popusti = await this.http.get<Popust[]>(MojConfig.adresa_servera+"GetPopustiGost",
      {headers:{"my-token": this.auth.getToken()!.token!}}).toPromise();
  }
  async getGradovi () {
    this.gradovi = await this.http.get<Grad[]>(MojConfig.adresa_servera+"GetGradovi").toPromise();
  }
  async ngOnInit() {
    await this.ucitajKorisnika();
    await this.getPopusti();
    await this.getGradovi();
    await this.getRezervacije();
    console.log(window.innerHeight);
  }
  ngOnDestroy() {
    this.clearInterval(this.interval);
  }

  async editKorisnik () {
    if(this.provjeriPolja()) {
      return;
    }
    if(await this.postojiEmail()) {
      return;
    }
    if(await this.postojiUsername()) {
      return;
    }
    if(await this.postojiBrojTelefona()) {
      return;
    }
    let edit :KorisnikEditRequest = {
      ime: this.ime,
      prezime:this.prezime,
      datumRodjenja : new Date(this.datumRodjenja),
      gradID : this.gradId,
      email:this.email,
      brojTelefona:this.brojTelefona,
      username:this.username
    };
    let res = await this.http.post<KorisnikEditResponse>(MojConfig.adresa_servera+"EditGost",edit,
      {headers:{"my-token":this.auth.getToken()?.token!} }).toPromise();
    if(res!.editovan) {
      if(!isNullOrEmpty(res?.greska!)) {
        alert(res?.greska!);
      }
      if(this.promijenjenaSlika) {
        let obj = {
          base64String: this.slikaURL,
          korisnikId: this.auth.getToken()?.gost?.korisnikID
        };
        let res = await this.http.post<KorisnikEditResponse>(MojConfig.adresa_servera+"SendProfilna", obj,
          {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();
        if(!res?.editovan) {
          alert("Ucitavanje slike nije uspjelo! Greska: " + res?.greska);
        }
        else {
          location.reload();
        }
      }
      await this.ucitajKorisnika();
      this.upozorenje="";
      this.dialog_edit= false;
    }
    else {
      this.upozorenje=res?.greska!;
    }
  }
  popuni() {
    this.ime = this.korisnik!.ime;
    this.prezime = this.korisnik!.prezime;
    this.email = this.korisnik!.email;
    this.username = this.korisnik!.username;
    this.gradId = this.korisnik!.gradID;
    this.brojTelefona = this.korisnik!.brojTelefona;
    this.datumRodjenja = this.korisnik!.datumRodjenja.toString().split('T')[0];
  }
  boolstyle (stanje:boolean) {
    if(stanje) return {"color":"green"};
    else return {"color" :"red"};
  }
  promijeniBroj (broj:number) {
    this.brojZvjezdica=broj;
    this.listaObicnih=[];
    this.listaDrugih = [];
    for(let i =0; i<broj; i++) {
      this.listaObicnih.push(i+1);
    }
    for(let i =broj; i<5;i++) {
      this.listaDrugih.push(i+1);
    }
  }
  async posaljiRecenziju() {
    if(this.brojZvjezdica==0) {
      alert("Niste unijeli ocjenu!");
      return;
    }
    let rec:RecenzijaPost = {
      rezervacijaId:this.rezervacijaId,
      komentar:this.komentar,
      ocjena : this.brojZvjezdica
    };
    let res = await this.http.post(MojConfig.adresa_servera+"DodajRecenziju",rec,
      {headers:{"my-token":this.auth.getToken()?.token!} }).toPromise();
    if(res) {
      alert("Uspjesno poslana recenzija!");
      this.dialog_recenzija =!this.dialog_recenzija;
      await this.getRezervacije();
    }
    else {
      alert("Neuspjesno poslana recenzija!");
    }
  }
  async promjenaLozinke() {
    if(this.isNullOrEmpty(this.novaLozinka) || this.isNullOrEmpty(this.staraLozinka)) {
     this.lozinka_upozorenje= "Sva polja su obavezna!";
     return;
    }
    if(this.novaLozinka.length<8 || this.staraLozinka.length<8) {
      this.lozinka_upozorenje = "Vasa lozinka mora sadrzavati minimalno 8 karaktera!";
      return;
    }
    let obj: LozinkaRequest = {
         staraLozinka: this.staraLozinka,
         novaLozinka:this.novaLozinka
    };
     let res = await this.http.post<LozinkaResponse>(MojConfig.adresa_servera+"PromjenaLozinke",
      obj,{headers:{"my-token":this.auth.getToken()?.token!}} ).toPromise();
     if(res!.promijenjena) {
       alert("Uspjesno promijenjena lozinka!");
       this.dialog_lozinka = !this.dialog_lozinka;
     }
     else {
       this.staraLozinka="";
       this.novaLozinka="";
       this.lozinka_upozorenje=res!.greska;
     }
  }
  async izbrisiNalog() {
    let res : BrisanjeNalogaResponse = await this.http.get<BrisanjeNalogaResponse>(MojConfig.adresa_servera+"BrisanjeNaloga",
      {headers:{"my-token": this.auth.getToken()?.token!}}).toPromise() as BrisanjeNalogaResponse;
    if(res.obrisan) {
      localStorage.removeItem("my-token");
      this.router.navigate(["/login"]);
      return;
    }
    else {
      alert("Greska: "+res.greska);
      this.brisanje_naloga_dialog =!this.brisanje_naloga_dialog;
      return;
    }
  }
  protected readonly event = event;
  protected readonly clearInterval = clearInterval;
}
