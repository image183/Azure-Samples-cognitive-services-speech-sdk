//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
// uspcommon.h: common definitions and declaration used by USP internal implementation
//

#pragma once

#include "usperror.h"

#define UNUSED(x) (void)(x)

#define USP_RETURN_NOT_IMPLEMENTED() \
    do { \
        LogError("Not implemented"); \
        return USP_NOT_IMPLEMENTED; \
    } while (0)

#define USE_BUFFER_SIZE    ((size_t)-1)

#if defined _MSC_VER
#define PROTOCOL_VIOLATION(__fmt, ...)  LogError("ProtocolViolation:" __fmt, __VA_ARGS__)
#else
#define PROTOCOL_VIOLATION(__fmt, ...)  LogError("ProtocolViolation:" __fmt, ##__VA_ARGS__)
#endif

typedef struct ProxyServerInfo
{
    const char* host;
    int port;
    const char* username;
    const char* password;
} ProxyServerInfo;

#ifdef __cplusplus

#include <string>

namespace Microsoft {
namespace CognitiveServices {
namespace Speech {
namespace USP {

    namespace endpoint
    {
        const std::string protocol = "wss://";

        namespace unifiedspeech
        {
            const std::string hostnameSuffix = ".stt.speech.microsoft.com";
            const std::string pathPrefix = "/speech/recognition/";
            const std::string pathSuffix = "/cognitiveservices/v1";
            const std::string langQueryParam = "language=";
            const std::string deploymentIdQueryParam = "cid=";
            const std::string outputFormatQueryParam = "format=";
        }

        namespace translation
        {
            const std::string hostnameSuffix = ".s2s.speech.microsoft.com";
            const std::string path = "/speech/translation/cognitiveservices/v1";
            const std::string from = "from=";
            const std::string to = "to=";
            const std::string voice = "voice=";
            const std::string features = "features=";
            const std::string requireVoice = "texttospeech";
        }

        namespace luis
        {
            const std::string hostname = "speech.platform.bing.com";
            const std::string pathPrefix1 = "/speech/";
            const std::string pathPrefix2 = "/recognition/";
            const std::string pathSuffix = "/cognitiveservices/v1";
            const std::string langQueryParam = "language=";
        }
            
        namespace CDSDK
        {
            const std::string url = "speech.platform.bing.com/cortana/api/v1?environment=Home&";
        }
    }

    namespace path {
        const std::string speechHypothesis = "speech.hypothesis";
        const std::string speechPhrase = "speech.phrase";
        const std::string speechFragment = "speech.fragment";
        const std::string turnStart = "turn.start";
        const std::string turnEnd = "turn.end";
        const std::string speechStartDetected = "speech.startDetected";
        const std::string speechEndDetected = "speech.endDetected";
        const std::string translationHypothesis = "translation.hypothesis";
        const std::string translationPhrase = "translation.phrase";
        const std::string translationSynthesis = "translation.synthesis";
        const std::string translationSynthesisEnd = "translation.synthesis.end";
        const std::string audio = "audio";
    }
    //Todo: Figure out what to do about user agent build hash and version number
    const auto g_userAgent = "CortanaSDK (Windows;Win32;DeviceType=Near;SpeechClient=2.0.4)";

    namespace headers {
        const auto userAgent = "User-Agent";
        const auto ocpApimSubscriptionKey = "Ocp-Apim-Subscription-Key";
        const auto authorization = "Authorization";
        const auto searchDelegationRPSToken = "X-Search-DelegationRPSToken";
        const auto audioResponseFormat = "X-Output-AudioCodec";
        const auto contentType = "Content-Type";
        const auto streamId = "X-StreamId";
        const auto requestId = "X-RequestId";
    }

    namespace json_properties {
        const std::string offset = "Offset";
        const std::string duration = "Duration";
        const std::string text = "Text";
        const std::string recoStatus = "RecognitionStatus";
        const std::string displayText = "DisplayText";
        const std::string context = "context";
        const std::string tag = "serviceTag";

        const std::string nbest = "NBest";
        const std::string confidence = "Confidence";
        const std::string display = "Display";

        const std::string translation = "Translation";
        const std::string translationStatus = "TranslationStatus";
        const std::string translationFailureReason = "FailureReason";
        const std::string translations = "Translations";
        const std::string synthesisStatus = "SynthesisStatus";
        const std::string lang = "Language";
    }

}
}
}
}
#else

extern const char* g_keywordContentType;
extern const char* g_messagePathSpeechHypothesis;
extern const char* g_messagePathSpeechPhrase;
extern const char* g_messagePathSpeechFragment;
extern const char* g_messagePathTurnStart;
extern const char* g_messagePathTurnEnd;
extern const char* g_messagePathSpeechEndDetected;
extern const char* g_messagePathSpeechStartDetected;

#endif