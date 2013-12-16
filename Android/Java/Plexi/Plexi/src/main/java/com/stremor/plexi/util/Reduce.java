package com.stremor.plexi.util;

import java.util.Collection;

/**
 * Created by jeffschifano on 10/31/13.
 */

/*
reduce: (fun, iterable, initial) =>
		if iterable.length > 0
			initial = fun(initial, iterable[0])
			return @reduce(fun, iterable.slice(1), initial)
		else
			return initial

 */

public abstract class Reduce<Iterable, Initial> {
    public abstract Initial function(Initial a, Iterable b);

    public Object apply(Collection<Iterable> iterable, Initial initial) {
        if (!iterable.isEmpty()) {
            Iterable next = iterable.iterator().next();
            initial = this.function(initial, next);

            iterable.iterator().remove();

            return this.apply(iterable, initial);
        } else {
            return initial;
        }

        /*
        Initial value = null;
        for (Iterable next : iterable) {
            if (value == null) {
                value = next;
            } else {
                value = this.function(value, next);
            }
        }
        return value;
        */
    }
}
