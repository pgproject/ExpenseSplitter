
const currencyCodes: Record<number, string> = {
  0: "PLN",
  1: "EUR",
  2: "USD",
};

export function getCurrencyCode(currency: number): string {
  return currencyCodes[currency] ?? "PLN";
}

export function formatAmount(amount: number, currency: number): string {
  const currencyCode = currencyCodes[currency] ?? "PLN";

  return new Intl.NumberFormat("pl-PL", {
    style: "currency",
    currency: currencyCode,
  }).format(amount);
}

export function formatDate(date: string): string {
  return new Date(date).toLocaleDateString("pl-PL");
}
