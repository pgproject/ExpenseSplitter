export const Currency = {
  PLN: 0,
  EUR: 1,
  USD: 2,
} as const;

export type Currency = typeof Currency[keyof typeof Currency];
