package com.stremor.plexi.interfaces;

import com.stremor.plexi.models.Choice;
import com.stremor.plexi.models.LoginResponse;
import com.stremor.plexi.models.ShowModel;
import com.stremor.plexi.models.SignupResponse;

/**
 * Classes implementing this interface are expected to be the bridge between the user and the Plexi
 * backend. These classes provide implementations for interacting with the user and retrieving user
 * input, and optionally for handling events detected by Plexi.
 */
public interface IPlexiListener {
    /// ACTIONS ///

    /**
     * Show a response to the user.
     *
     * @param show An object describing the content to be shown.
     * @param speak A natural-language string which can be spoken to the user.
     */
    public void show(ShowModel show, String speak);

    /**
     * Present a choice of values to the user. When the user has completed a selection, the client
     * should call
     * {@link com.stremor.plexi.interfaces.IPlexiService#choice(com.stremor.plexi.models.Choice)}
     * with the chosen string result.
     *
     * @param choices
     */
    public void requestChoice(Choice[] choices);

    /// EVENTS ///

    /**
     * Handle a login response.
     *
     * @param response
     */
    public void onLoginResponse(LoginResponse response);

    /**
     * Handle a signup response.
     *
     * @param response
     */
    public void onSignupResponse(SignupResponse response);

    /**
     * Handle an error detected by the Plexi backend.
     *
     * @param message A natural-language message describing the error.
     */
    public void onError(String message);

    /**
     * Handle an internal error experienced by the Plexi backend.
     */
    public void onInternalError();
}
