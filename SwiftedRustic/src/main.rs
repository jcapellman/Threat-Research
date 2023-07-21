use std::env;

pub use crate::swifto;

fn parse(args: Vec<String>) -> i16 {
    if args.len() == 1 {
        return 5
    }

    for arg in args.iter().skip(1) {
        return arg.parse().expect("Not a number!");
    }

    return 5
}

fn main() {
    let delaySeconds: i16 = parse(env::args().collect());

    swift(delaySeconds);
}
