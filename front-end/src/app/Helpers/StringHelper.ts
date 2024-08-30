export function isNullOrEmpty(tekst:string) {
  if(tekst =='') return true;
  let lista = tekst.split(" ");
  let text = lista.join('');
  return text.length==0;
}
export function formatirajBroj(n:number) {
  return n <10 ? '0'+n : n;
}
