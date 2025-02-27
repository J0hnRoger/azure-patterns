import { Button } from "@/components/ui/button"
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form"
import { Input } from "@/components/ui/input"
import { zodResolver } from "@hookform/resolvers/zod"
import { useForm } from "react-hook-form"
import { z } from "zod"

const formSchema = z.object({
    message: z.string().min(1, {
        message: "Message is required",
    }),
})

interface IAFormProps {
    onSubmit: (query: string) => void; // Notez que ce n'est plus une Promise
    isLoading: boolean;
}

export const IAForm = ({ onSubmit, isLoading }: IAFormProps) => {

    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            message: "",
        },
    })

    function handleSubmit(values: z.infer<typeof formSchema>) {
        onSubmit(values.message);
        form.reset(); // Réinitialise le formulaire après l'envoi
    }

    return (
        <div className="flex flex-col w-full h-full gap-4">
            <Form {...form}>
                <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-8 w-full">
                    <FormField
                        control={form.control}
                        name="message"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Message</FormLabel>
                                <FormControl>
                                    <Input placeholder="..." {...field} />
                                </FormControl>
                                <FormDescription>
                                    Posez-moi une question
                                </FormDescription>
                                <FormMessage />
                            </FormItem>
                        )}
                    />
                    <Button type="submit" disabled={isLoading}>
                        {isLoading ? "Envoi en cours..." : "Envoyer"}
                    </Button>

                </form>
            </Form>
        </div>
    )
}
