// Maps a quote status to a VcStatus variant; same buckets as the Quote module's list, so both lists look alike.
type Variant = "info" | "warning" | "danger" | "success" | "primary";
const buckets: Record<Variant, string[]> = {
  primary: ["new", "draft"],
  warning: ["processing", "proposal sent"],
  success: ["ordered"],
  danger: ["cancelled", "canceled", "declined", "expired"],
  info: [],
};
export function statusVariant(status?: string): Variant {
  const value = (status ?? "").trim().toLowerCase();
  return (Object.keys(buckets) as Variant[]).find((k) => buckets[k].includes(value)) ?? "info";
}

/** Message of a failed request, for showError. */
export const errorText = (e: unknown) => String((e as Error)?.message ?? e);
