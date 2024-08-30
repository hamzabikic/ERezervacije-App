export interface GostResponse extends KorisnikResponse{
  brojRezervacija:number;
  datumRegistracije:string;
  aktivan:boolean;
  verifikovanEmail:boolean;
  verifikovanTelefon:boolean;
}
export interface KorisnikResponse {
  ime: string;
  prezime: string;
  username: string;
  email: string;
  brojTelefona: string;
  datumRodjenja: Date; // Consider using Date type if you're dealing with actual date objects
  gradID: number;
  grad:Grad;
}
export interface UposlenikResponse extends KorisnikResponse {
  datumZaposlenja:Date;
  strucnaSprema:string;
}
export interface Grad {
  gradID: number;
  naziv: string;
  ptt: number;
  drzava: string;
}
export interface KorisnikEditRequest {
  ime: string;
  prezime: string;
  username: string;
  email: string;
  brojTelefona: string;
  datumRodjenja:Date;
  gradID: number;
}
export interface Popust {
  popustID:number;
  procenat:number;
  razlog:string;
}
export interface RezervacijaGostResponse {
  rezervacijaID: number;
  datumRezervacije: string;
  vrijemeRezervacije: string;
  stolovi: number[];
  odobrena: boolean;
  preuzeta: boolean;
  recenzirano:boolean;
  ponistena:boolean;
  otkazana:boolean;
}
export interface RecenzijaPost {
  rezervacijaId:number;
  komentar:string;
  ocjena:number;
}
export interface GostRezervacijaInfo {
  rezervacijaId: number;
  datum: Date;
  vrijeme: number;
  trajanje: number;
  posebneZelje: string;
  stolovi: Stol[];
  komentar:string;
  ponistena:boolean;
}
export interface Stol {
  stolId: number;
  brojStola: number;
  jeUnutra: boolean;
  jePusackaZona: boolean;
  brojStolica: number;
}
export interface PopustResponse {
  popustId:number;
  imePrezime:string;
  iznos:number;
  razlog:string;
}
export interface RezervacijaHostesaResponse extends  RezervacijaGostResponse{
  imePrezime:string;
  brojTelefona:string;
}
export interface LozinkaResponse {
  promijenjena:boolean;
  greska:string;
}
export interface LozinkaRequest {
  staraLozinka: string;
  novaLozinka:string;
}
export interface RecenzijaManagerResponse {
  rezervacijaId:number;
  datum:string;
  vrijeme:string;
  imePrezime:string;
  brojTelefona:string;
  ocjena:number;
  recenzijaId:number;
}
export interface RecenzijaInfoManager extends  RecenzijaManagerResponse {
  komentar:string;
  stolovi: Stol[];
  posebneZelje:string;
}
export interface BrisanjeNalogaResponse {
  obrisan:boolean;
  greska:string;
}
