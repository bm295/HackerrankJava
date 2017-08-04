class MyRegex {
    private String part = "((25[0-5])|(2[0-4][0-9])|([0-1]{0,1}[0-9]{1,2}))";
    String pattern = String.format("%s\\.%s\\.%s\\.%s", part, part, part, part);
}
