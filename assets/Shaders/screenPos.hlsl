vec2 getScreenPos() {
    return gl_FragCoord.xy / screenRes;
}