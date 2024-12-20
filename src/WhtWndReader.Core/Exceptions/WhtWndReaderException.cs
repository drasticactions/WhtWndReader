// <copyright file="WhtWndReaderException.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using FishyFlip.Models;

namespace WhtWndReader.Exceptions;

/// <summary>
/// Represents an exception specific to the WhtWndReader.iOS application.
/// </summary>
public class WhtWndReaderException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WhtWndReaderException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public WhtWndReaderException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WhtWndReaderException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="ex">The exception that is the cause of the current exception.</param>
    public WhtWndReaderException(string message, Exception ex)
        : base(message, ex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WhtWndReaderException"/> class with a specified error message and an ATError object.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="atError">The ATError object that contains additional error information.</param>
    public WhtWndReaderException(string message, ATError atError)
        : base(message)
    {
        this.AtError = atError;
    }

    /// <summary>
    /// Gets the ATError object that contains additional error information.
    /// </summary>
    public ATError? AtError { get; init; }
}