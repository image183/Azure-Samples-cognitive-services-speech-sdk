package tests.unit;
//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

import org.junit.runner.RunWith;
import org.junit.runners.Suite;
import org.junit.runners.Suite.SuiteClasses;

@RunWith(Suite.class)
@SuiteClasses({
    SpeechConfigTests.class,
    SpeechRecognizerTests.class,
    IntentRecognizerTests.class,
    TranslationRecognizerTests.class,
    KeywordRecognitionModelTests.class,
    LanguageUnderstandingModelTests.class,
    RecognizerTests.class,
    SpeechPropertyIdTests.class,
    AudioInputStreamTests.class,
    PropertyCollectionTests.class
    })
public class AllUnitTests {

}