//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.CognitiveServices.Speech.Internal;
using static Microsoft.CognitiveServices.Speech.Internal.SpxExceptionThrower;
using System.Threading;

namespace Microsoft.CognitiveServices.Speech
{
    /// <summary>
    /// Performs speech recognition from microphone, file, or other audio input streams, and gets transcribed text as result.
    /// </summary>
    /// <example>
    /// An example to use the speech recognizer from microphone and listen to events generated by the recognizer.
    /// <code>
    /// public async Task SpeechContinuousRecognitionAsync()
    /// {
    ///     // Creates an instance of a speech config with specified subscription key and service region.
    ///     // Replace with your own subscription key and service region (e.g., "westus").
    ///     var config = SpeechConfig.FromSubscription("YourSubscriptionKey", "YourServiceRegion");
    ///
    ///     // Creates a speech recognizer from microphone.
    ///     using (var recognizer = new SpeechRecognizer(config))
    ///     {
    ///         // Subscribes to events.
    ///         recognizer.Recognizing += (s, e) => {
    ///             Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
    ///         };
    ///
    ///         recognizer.Recognized += (s, e) => {
    ///             var result = e.Result;
    ///             Console.WriteLine($"Reason: {result.Reason.ToString()}");
    ///             if (result.Reason == ResultReason.RecognizedSpeech)
    ///             {
    ///                     Console.WriteLine($"Final result: Text: {result.Text}.");
    ///             }
    ///         };
    ///
    ///         recognizer.Canceled += (s, e) => {
    ///             Console.WriteLine($"\n    Recognition Canceled. Reason: {e.Reason.ToString()}, CanceledReason: {e.Reason}");
    ///         };
    ///
    ///         recognizer.SessionStarted += (s, e) => {
    ///             Console.WriteLine("\n    Session started event.");
    ///         };
    ///
    ///         recognizer.SessionStopped += (s, e) => {
    ///             Console.WriteLine("\n    Session stopped event.");
    ///         };
    ///
    ///         // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
    ///         await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
    ///
    ///         do
    ///         {
    ///             Console.WriteLine("Press Enter to stop");
    ///         } while (Console.ReadKey().Key != ConsoleKey.Enter);
    ///
    ///         // Stops recognition.
    ///         await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
    ///     }
    /// }
    /// </code>
    /// </example>
    public sealed class SpeechRecognizer : Recognizer
    {
        private event EventHandler<SpeechRecognitionEventArgs> _Recognizing;
        private event EventHandler<SpeechRecognitionEventArgs> _Recognized;
        private event EventHandler<SpeechRecognitionCanceledEventArgs> _Canceled;

        /// <summary>
        /// The event <see cref="Recognizing"/> signals that an intermediate recognition result is received.
        /// </summary>
        public event EventHandler<SpeechRecognitionEventArgs> Recognizing
        {
            add
            {
                if (this._Recognizing == null)
                {
                    ThrowIfFail(Internal.Recognizer.recognizer_recognizing_set_callback(recoHandle, recognizingCallbackDelegate, GCHandle.ToIntPtr(gch)));
                }
                this._Recognizing += value;
            }
            remove
            {
                this._Recognizing -= value;
                if (this._Recognizing == null)
                {
                    LogErrorIfFail(Internal.Recognizer.recognizer_recognizing_set_callback(recoHandle, null, IntPtr.Zero));
                }
            }
        }

        /// <summary>
        /// The event <see cref="Recognized"/> signals that a final recognition result is received.
        /// </summary>
        public event EventHandler<SpeechRecognitionEventArgs> Recognized
        {
            add
            {
                if (this._Recognized == null)
                {
                    ThrowIfFail(Internal.Recognizer.recognizer_recognized_set_callback(recoHandle, recognizedCallbackDelegate, GCHandle.ToIntPtr(gch)));
                }
                this._Recognized += value;
            }
            remove
            {
                this._Recognized -= value;
                if (this._Recognized == null)
                {
                    LogErrorIfFail(Internal.Recognizer.recognizer_recognized_set_callback(recoHandle, null, IntPtr.Zero));
                }
            }
        }

        /// <summary>
        /// The event <see cref="Canceled"/> signals that the speech recognition was canceled.
        /// </summary>
        public event EventHandler<SpeechRecognitionCanceledEventArgs> Canceled
        {
            add
            {
                if (this._Canceled == null)
                {
                    ThrowIfFail(Internal.Recognizer.recognizer_canceled_set_callback(recoHandle, canceledCallbackDelegate, GCHandle.ToIntPtr(gch)));
                }
                this._Canceled += value;
            }
            remove
            {
                this._Canceled -= value;
                if (this._Canceled == null)
                {
                    LogErrorIfFail(Internal.Recognizer.recognizer_canceled_set_callback(recoHandle, null, IntPtr.Zero));
                }
            }
        }

        private CallbackFunctionDelegate recognizingCallbackDelegate;
        private CallbackFunctionDelegate recognizedCallbackDelegate;
        private CallbackFunctionDelegate canceledCallbackDelegate;

        /// <summary>
        /// Creates a new instance of SpeechRecognizer.
        /// </summary>
        /// <param name="speechConfig">Speech configuration</param>
        public SpeechRecognizer(SpeechConfig speechConfig)
            : this(FromConfig(SpxFactory.recognizer_create_speech_recognizer_from_config, speechConfig))
        {
        }

        /// <summary>
        /// Creates a new instance of SpeechRecognizer.
        /// </summary>
        /// <param name="speechConfig">Speech configuration</param>
        /// <param name="audioConfig">Audio configuration</param>
        public SpeechRecognizer(SpeechConfig speechConfig, Audio.AudioConfig audioConfig)
            : this(FromConfig(SpxFactory.recognizer_create_speech_recognizer_from_config, speechConfig, audioConfig))
        {
            this.audioConfig = audioConfig;
        }

        /// <summary>
        /// Creates a new instance of SpeechRecognizer.
        /// Added in 1.9.0
        /// </summary>
        /// <param name="speechConfig">Speech configuration</param>
        /// <param name="language">The source language</param>
        public SpeechRecognizer(SpeechConfig speechConfig, string language)
            : this(speechConfig, language, null)
        {
        }

        /// <summary>
        /// Creates a new instance of SpeechRecognizer.
        /// Added in 1.9.0
        /// </summary>
        /// <param name="speechConfig">Speech configuration</param>
        /// <param name="language">The source language</param>
        /// <param name="audioConfig">Audio configuration</param>
        public SpeechRecognizer(SpeechConfig speechConfig, string language, Audio.AudioConfig audioConfig)
            : this(speechConfig, SourceLanguageConfig.FromLanguage(language), audioConfig)
        {
            this.audioConfig = audioConfig;
        }

        /// <summary>
        /// Creates a new instance of SpeechRecognizer.
        /// Added in 1.9.0
        /// </summary>
        /// <param name="speechConfig">Speech configuration</param>
        /// <param name="sourceLanguageConfig">The source language config</param>
        public SpeechRecognizer(SpeechConfig speechConfig, SourceLanguageConfig sourceLanguageConfig)
            : this(speechConfig, sourceLanguageConfig, null)
        {
        }

        /// <summary>
        /// Creates a new instance of SpeechRecognizer.
        /// Added in 1.9.0
        /// </summary>
        /// <param name="speechConfig">Speech configuration</param>
        /// <param name="sourceLanguageConfig">The source language config</param>
        /// <param name="audioConfig">Audio configuration</param>
        public SpeechRecognizer(SpeechConfig speechConfig, SourceLanguageConfig sourceLanguageConfig, Audio.AudioConfig audioConfig)
            : this(FromConfig(SpxFactory.recognizer_create_speech_recognizer_from_source_lang_config, speechConfig, sourceLanguageConfig, audioConfig))
        {
            this.audioConfig = audioConfig;
        }

        /// <summary>
        /// Creates a new instance of SpeechRecognizer.
        /// Added in 1.9.0
        /// </summary>
        /// <param name="speechConfig">Speech configuration</param>
        /// <param name="autoDetectSourceLanguageConfig">The auto detect source language config</param>
        public SpeechRecognizer(SpeechConfig speechConfig, AutoDetectSourceLanguageConfig autoDetectSourceLanguageConfig)
            : this(speechConfig, autoDetectSourceLanguageConfig, null)
        {
        }

        /// <summary>
        /// Creates a new instance of SpeechRecognizer.
        /// Added in 1.9.0
        /// </summary>
        /// <param name="speechConfig">Speech configuration</param>
        /// <param name="autoDetectSourceLanguageConfig">The auto detect source language config</param>
        /// <param name="audioConfig">Audio configuration</param>
        public SpeechRecognizer(SpeechConfig speechConfig, AutoDetectSourceLanguageConfig autoDetectSourceLanguageConfig, Audio.AudioConfig audioConfig)
            : this(FromConfig(SpxFactory.recognizer_create_speech_recognizer_from_auto_detect_source_lang_config, speechConfig, autoDetectSourceLanguageConfig, audioConfig))
        {
            this.audioConfig = audioConfig;
        }

        internal SpeechRecognizer(InteropSafeHandle recoHandle) : base(recoHandle)
        {
            ThrowIfNull(recoHandle, "Invalid recognizer handle");

            recognizingCallbackDelegate = FireEvent_Recognizing;
            recognizedCallbackDelegate = FireEvent_Recognized;
            canceledCallbackDelegate = FireEvent_Canceled;

            IntPtr propertyHandle = IntPtr.Zero;
            ThrowIfFail(Internal.Recognizer.recognizer_get_property_bag(recoHandle, out propertyHandle));
            Properties = new PropertyCollection(propertyHandle);
        }

        /// <summary>
        /// Gets the endpoint ID of a customized speech model that is used for speech recognition.
        /// </summary>
        /// <returns>the endpoint ID of a customized speech model that is used for speech recognition</returns>
        public string EndpointId
        {
            get
            {
                return Properties.GetProperty(PropertyId.SpeechServiceConnection_EndpointId);
            }
        }

        /// <summary>
        /// Gets/sets authorization token used to communicate with the service.
        /// Note: The caller needs to ensure that the authorization token is valid. Before the authorization token
        /// expires, the caller needs to refresh it by calling this setter with a new valid token.
        /// Otherwise, the recognizer will encounter errors during recognition.
        /// </summary>
        public string AuthorizationToken
        {
            get
            {
                return Properties.GetProperty(PropertyId.SpeechServiceAuthorization_Token);
            }

            set
            {
                if(value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                Properties.SetProperty(PropertyId.SpeechServiceAuthorization_Token, value);
            }
        }

        /// <summary>
        /// Gets the language name that was set when the recognizer was created.
        /// </summary>
        public string SpeechRecognitionLanguage
        {
            get
            {
                return Properties.GetProperty(PropertyId.SpeechServiceConnection_RecoLanguage, string.Empty);
            }
        }

        /// <summary>
        /// Gets the output format setting.
        /// </summary>
        public OutputFormat OutputFormat
        {
            get
            {
                return Properties.GetProperty(PropertyId.SpeechServiceResponse_RequestDetailedResultTrueFalse, "false") == "true"
                    ? OutputFormat.Detailed
                    : OutputFormat.Simple;
            }
        }

        /// <summary>
        /// The collection of properties and their values defined for this <see cref="SpeechRecognizer"/>.
        /// Note: The property collection is only valid until the recognizer owning this Properties is disposed or finalized.
        /// </summary>
        public PropertyCollection Properties { get; internal set; }

        /// <summary>
        /// Starts speech recognition, and returns after a single utterance is recognized. The end of a
        /// single utterance is determined by listening for silence at the end or until a maximum of 15
        /// seconds of audio is processed.  The task returns the recognition text as result.
        /// Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
        /// shot recognition like command or query.
        /// For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
        /// </summary>
        /// <returns>A task representing the recognition operation. The task returns a value of <see cref="SpeechRecognitionResult"/> </returns>
        /// <example>
        /// The following example creates a speech recognizer, and then gets and prints the recognition result.
        /// <code>
        /// public async Task SpeechSingleShotRecognitionAsync()
        /// {
        ///     // Creates an instance of a speech config with specified subscription key and service region.
        ///     // Replace with your own subscription key and service region (e.g., "westus").
        ///     var config = SpeechConfig.FromSubscription("YourSubscriptionKey", "YourServiceRegion");
        ///
        ///     // Creates a speech recognizer using microphone as audio input. The default language is "en-us".
        ///     using (var recognizer = new SpeechRecognizer(config))
        ///     {
        ///         Console.WriteLine("Say something...");
        ///
        ///         // Starts speech recognition, and returns after a single utterance is recognized. The end of a
        ///         // single utterance is determined by listening for silence at the end or until a maximum of 15
        ///         // seconds of audio is processed.  The task returns the recognition text as result.
        ///         // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
        ///         // shot recognition like command or query.
        ///         // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
        ///         var result = await recognizer.RecognizeOnceAsync();
        ///
        ///         // Checks result.
        ///         if (result.Reason == ResultReason.RecognizedSpeech)
        ///         {
        ///             Console.WriteLine($"RECOGNIZED: Text={result.Text}");
        ///         }
        ///         else if (result.Reason == ResultReason.NoMatch)
        ///         {
        ///             Console.WriteLine($"NOMATCH: Speech could not be recognized.");
        ///         }
        ///         else if (result.Reason == ResultReason.Canceled)
        ///         {
        ///             var cancellation = CancellationDetails.FromResult(result);
        ///             Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");
        ///
        ///             if (cancellation.Reason == CancellationReason.Error)
        ///             {
        ///                 Console.WriteLine($"CANCELED: ErrorCode={cancelation.ErrorCode}");
        ///                 Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
        ///                 Console.WriteLine($"CANCELED: Did you update the subscription info?");
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        public Task<SpeechRecognitionResult> RecognizeOnceAsync()
        {
            return Task.Run(() =>
            {
                SpeechRecognitionResult result = null;
                base.DoAsyncRecognitionAction(() => result = new SpeechRecognitionResult(RecognizeOnce()));
                return result;
            });
        }

        /// <summary>
        /// Starts speech recognition on a continuous audio stream, until StopContinuousRecognitionAsync() is called.
        /// User must subscribe to events to receive recognition results.
        /// </summary>
        /// <returns>A task representing the asynchronous operation that starts the recognition.</returns>
        public Task StartContinuousRecognitionAsync()
        {
            return Task.Run(() =>
            {
                base.DoAsyncRecognitionAction(StartContinuousRecognition);
            });
        }

        /// <summary>
        /// Stops continuous speech recognition.
        /// </summary>
        /// <returns>A task representing the asynchronous operation that stops the recognition.</returns>
        public Task StopContinuousRecognitionAsync()
        {
            return Task.Run(() =>
            {
                base.DoAsyncRecognitionAction(StopContinuousRecognition);
            });
        }

        /// <summary>
        /// Starts speech recognition on a continuous audio stream with keyword spotting, until StopKeywordRecognitionAsync() is called.
        /// User must subscribe to events to receive recognition results.
        /// </summary>
        /// Note: Keyword spotting (KWS) functionality might work with any microphone type, official KWS support, however, is currently limited to the microphone arrays found in the Azure Kinect DK hardware or the Speech Devices SDK.
        /// <param name="model">The keyword recognition model that specifies the keyword to be recognized.</param>
        /// <returns>A task representing the asynchronous operation that starts the recognition.</returns>
        public Task StartKeywordRecognitionAsync(KeywordRecognitionModel model)
        {
            return Task.Run(() =>
            {
                base.DoAsyncRecognitionAction(() => StartKeywordRecognition(model));
            });
        }

        /// <summary>
        /// Stops continuous speech recognition with keyword spotting.
        /// </summary>
        /// Note: Keyword spotting (KWS) functionality might work with any microphone type, official KWS support, however, is currently limited to the microphone arrays found in the Azure Kinect DK hardware or the Speech Devices SDK.
        /// <returns>A task representing the asynchronous operation that stops the recognition.</returns>
        public Task StopKeywordRecognitionAsync()
        {
            return Task.Run(() =>
            {
                base.DoAsyncRecognitionAction(StopKeywordRecognition);
            });
        }

        ~SpeechRecognizer()
        {
            isDisposing = true;
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // This will make Properties unaccessible.
                Properties.Close();
            }

            if (recoHandle != null)
            {
                LogErrorIfFail(Internal.Recognizer.recognizer_recognizing_set_callback(recoHandle, null, IntPtr.Zero));
                LogErrorIfFail(Internal.Recognizer.recognizer_recognized_set_callback(recoHandle, null, IntPtr.Zero));
                LogErrorIfFail(Internal.Recognizer.recognizer_canceled_set_callback(recoHandle, null, IntPtr.Zero));
                LogErrorIfFail(Internal.Recognizer.recognizer_session_started_set_callback(recoHandle, null, IntPtr.Zero));
                LogErrorIfFail(Internal.Recognizer.recognizer_session_stopped_set_callback(recoHandle, null, IntPtr.Zero));
                LogErrorIfFail(Internal.Recognizer.recognizer_speech_start_detected_set_callback(recoHandle, null, IntPtr.Zero));
                LogErrorIfFail(Internal.Recognizer.recognizer_speech_end_detected_set_callback(recoHandle, null, IntPtr.Zero));
                recoHandle.Dispose();
            }

            recognizingCallbackDelegate = null;
            recognizedCallbackDelegate = null;
            canceledCallbackDelegate = null;

            base.Dispose(disposing);
        }

        private readonly Audio.AudioConfig audioConfig;

        // Defines private methods to raise a C# event for intermediate/final result when a corresponding callback is invoked by the native layer.
        [MonoPInvokeCallback(typeof(CallbackFunctionDelegate))]
        private static void FireEvent_Recognizing(IntPtr hreco, IntPtr hevent, IntPtr pvContext)
        {
            try
            {
                var recognizer = InteropSafeHandle.GetObjectFromWeakHandle<SpeechRecognizer>(pvContext);
                if (recognizer == null || recognizer.isDisposing)
                {
                    return;
                }
                var resultEventArg = new SpeechRecognitionEventArgs(hevent);
                recognizer._Recognizing?.Invoke(recognizer, resultEventArg);
            }
            catch (InvalidOperationException)
            {
                LogError(Internal.SpxError.InvalidHandle);
            }
        }

        [MonoPInvokeCallback(typeof(CallbackFunctionDelegate))]
        private static void FireEvent_Recognized(IntPtr hreco, IntPtr hevent, IntPtr pvContext)
        {
            try
            {
                var recognizer = InteropSafeHandle.GetObjectFromWeakHandle<SpeechRecognizer>(pvContext);
                if (recognizer == null || recognizer.isDisposing)
                {
                    return;
                }
                var resultEventArg = new SpeechRecognitionEventArgs(hevent);
                recognizer._Recognized?.Invoke(recognizer, resultEventArg);
            }
            catch (InvalidOperationException)
            {
                LogError(Internal.SpxError.InvalidHandle);
            }
        }

        [MonoPInvokeCallback(typeof(CallbackFunctionDelegate))]
        private static void FireEvent_Canceled(IntPtr hreco, IntPtr hevent, IntPtr pvContext)
        {
            try
            {
                var recognizer = InteropSafeHandle.GetObjectFromWeakHandle<SpeechRecognizer>(pvContext);
                if (recognizer == null || recognizer.isDisposing)
                {
                    return;
                }
                var resultEventArg = new SpeechRecognitionCanceledEventArgs(hevent);
                recognizer._Canceled?.Invoke(recognizer, resultEventArg);
            }
            catch (InvalidOperationException)
            {
                LogError(Internal.SpxError.InvalidHandle);
            }
        }
    }
}
