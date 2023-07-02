// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    //public class BookingDialog : CancelAndHelpDialog
    //{
    //    private const string DestinationStepMsgText = "Where would you like to travel to?";
    //    private const string OriginStepMsgText = "Where are you traveling from?";

    //    public BookingDialog()
    //        : base(nameof(BookingDialog))
    //    {

    public class BookingDialog : CancelAndHelpDialog
    {

        private const string ComprarStepMsgText = "Qué te gustaría Comprar?";
        private const string Comprar2StepMsgText = "Qué marca te gustaría comprar?";


        public BookingDialog()
            : base(nameof(BookingDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                EquipoStepAsync,
                Equipo2StepAsync,
               // OriginStepAsync,
                MarcaStepAsync,
                ConfirmStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> EquipoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = (BookingDetails)stepContext.Options;

            if (bookingDetails.Equipo == null)
            {
                var promptMessage = MessageFactory.Text(ComprarStepMsgText, ComprarStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(bookingDetails.Equipo, cancellationToken);
        }
        private async Task<DialogTurnResult> Equipo2StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = (BookingDetails)stepContext.Options;

            if (bookingDetails.Marca == null)
            {
                var promptMessage = MessageFactory.Text(Comprar2StepMsgText, Comprar2StepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(bookingDetails.Equipo, cancellationToken);
        }


        //private async Task<DialogTurnResult> OriginStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    var bookingDetails = (BookingDetails)stepContext.Options;

        //    bookingDetails.Equipo = (string)stepContext.Result;

        //if (bookingDetails.Origin == null)
        //{
        //    var promptMessage = MessageFactory.Text(OriginStepMsgText, OriginStepMsgText, InputHints.ExpectingInput);
        //    return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        //}

        //return await stepContext.NextAsync(bookingDetails.Origin, cancellationToken);
        //}

        private async Task<DialogTurnResult> MarcaStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = (BookingDetails)stepContext.Options;

            // bookingDetails.Origin = (string)stepContext.Result;

            if (bookingDetails.Marca == null || IsAmbiguous(bookingDetails.Marca))
            {
                return await stepContext.BeginDialogAsync(nameof(DateResolverDialog), bookingDetails.Marca, cancellationToken);
            }

            return await stepContext.NextAsync(bookingDetails.Marca, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = (BookingDetails)stepContext.Options;

            //bookingDetails.TravelDate = (string)stepContext.Result;

            var messageText = $"Confirma que quiere reservar una cita para comprar una computadora marca Lenovo el día juli 2, 2023, es conforme?";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                var bookingDetails = (BookingDetails)stepContext.Options;

                return await stepContext.EndDialogAsync(bookingDetails, cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private static bool IsAmbiguous(string timex)
        {
            var timexProperty = new TimexProperty(timex);
            return !timexProperty.Types.Contains(Constants.TimexTypes.Definite);
        }
    }
}
