package com.stremor.plexi.interfaces;

import com.stremor.plexi.models.ShowModel;

/**
 * Classes implementing this interface are expected to be the bridge between the user and the Plexi
 * backend. These classes provide implementations for interacting with the user and retrieving user
 * input.
 */
public interface IPlexiListener {
    /**
     * Show a response to the user.
     *
     * @param show An object describing the content to be shown.
     * @param speak A natural-language string which can be spoken to the user.
     */
    public void show(ShowModel show, String speak);

    /**
     * Handle an error detected by the Plexi backend.
     *
     * @param message A natural-language message describing the error.
     */
    public void error(String message);
}
