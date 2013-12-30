package com.stremor.plexi.util;

/**
 * Created by jon on 30.12.2013.
 */
public class AsyncTaskResult<T> {
    private T result;
    private Exception exception;

    public AsyncTaskResult(T result) {
        this.result = result;
    }

    public AsyncTaskResult(Exception exception) {
        this.exception = exception;
    }

    public T getResult() {
        return result;
    }

    public Exception getException() {
        return exception;
    }
}
